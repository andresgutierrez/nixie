
using System.Collections.Concurrent;
using DotNext.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// Passes a message to the active actor reference making sure only one message is processed at a time.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ActorRunner<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class?
{
    private readonly ActorSystem actorSystem;

    private readonly ILogger? logger;

    private readonly ConcurrentQueue<ActorMessageReply<TRequest, TResponse>> inbox = new();
    
    private TaskCompletionSource? gracefulShutdown;

    private int processing = 1;

    private int shutdown = 1;

    /// <summary>
    /// The name/id of the actor.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Returns true if the actor's inbox is empty
    /// </summary>
    public bool IsEmpty => inbox.IsEmpty;

    /// <summary>
    /// Returns the number of messages in the inbox
    /// </summary>
    public int MessageCount => inbox.Count;

    /// <summary>
    /// The reference to the actor.
    /// </summary>
    public IActor<TRequest, TResponse>? Actor { get; set; }

    /// <summary>
    /// Reference to the current actor context
    /// </summary>
    public ActorContext<TActor, TRequest, TResponse>? ActorContext { get; set; }

    /// <summary>
    /// True if the actor is processing a message.
    /// </summary>
    public bool IsProcessing => processing == 0;

    /// <summary>
    /// True if the actor is shutdown
    /// </summary>
    public bool IsShutdown => shutdown == 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="actorSystem"></param>
    /// <param name="logger"></param>
    /// <param name="name"></param>
    public ActorRunner(ActorSystem actorSystem, ILogger? logger, string name)
    {
        this.actorSystem = actorSystem;
        this.logger = logger;

        Name = name;
    }

    /// <summary>
    /// Enqueues a message to the actor and tries to deliver it.
    /// The request/response type actors use an object to assign the response once completed. 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    /// <param name="parentReply"></param>
    /// <returns></returns>
    public TaskCompletionSource<TResponse?> SendAndTryDeliver(TRequest message, IGenericActorRef? sender, ActorMessageReply<TRequest, TResponse>? parentReply)
    {
        TaskCompletionSource<TResponse?> promise = new(TaskCreationOptions.RunContinuationsAsynchronously);

        ActorMessageReply<TRequest, TResponse> messageReply = parentReply ?? new(message, sender, promise);

        if (shutdown == 0)
        {
            promise.TrySetCanceled(CancellationToken.None);
            return promise;
        }

        //inbox.Enqueue(messageReply);

        //if (1 == Interlocked.Exchange(ref processing, 0))
        //    Task.Run(DeliverMessages);
        
        if (1 == Interlocked.Exchange(ref processing, 0))
        {
            if (inbox.IsEmpty)
                _ = DeliverSingleMessage(messageReply);
            else
            {
                inbox.Enqueue(messageReply);
                
                _ = DeliverMessages();
            }
        }
        else
            inbox.Enqueue(messageReply);

        return promise;
    }

    /// <summary>
    /// Try to shutdown the actor and returns a bool indicating success
    /// </summary>
    /// <returns></returns>
    public bool Shutdown()
    {
        bool success = 1 == Interlocked.Exchange(ref shutdown, 0);

        if (success)
            ActorContext?.PostShutdown();

        return success;
    }

    /// <summary>
    /// Tries to shutdown the actor returns a task whose result confirms shutdown within the specified timespan
    /// </summary>
    /// <param name="maxWait"></param>
    /// <returns></returns>
    public async ValueTask<bool> GracefulShutdown(TimeSpan maxWait)
    {
        if (inbox.IsEmpty)
            return Shutdown();

        if (gracefulShutdown is not null)
            return false;

        gracefulShutdown = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task timeout = Task.Delay(maxWait);

        Task completed = await Task.WhenAny(
            timeout,
            gracefulShutdown.Task
        );
        
        if (completed == timeout)
            Shutdown();

        return completed != timeout;
    }

    /// <summary>
    /// It retrieves a message from the inbox and invokes the actor by passing one message 
    /// at a time until the pending message list is cleared.
    /// </summary>
    /// <returns></returns>
    private async Task DeliverMessages()
    {
        try
        {
            if (Actor is null || ActorContext is null || shutdown == 0)
            {
                gracefulShutdown?.SetResult();
                return;
            }

            ActorContext.Runner = this;

            do
            {
                while (inbox.TryDequeue(out ActorMessageReply<TRequest, TResponse>? message))
                {
                    if (shutdown == 0 || ActorContext is null)
                        break;

                    if (message.Sender is not null)
                        ActorContext.Sender = message.Sender;
                    else
                        ActorContext.Sender = (IGenericActorRef)actorSystem.Nobody;

                    ActorContext.Reply = message;
                    ActorContext.ByPassReply = false;

                    try
                    {
                        TResponse? response = await Actor.Receive(message.Request);

                        if (!ActorContext.ByPassReply)
                            message.Promise.TrySetResult(response);
                    }
                    catch (Exception ex)
                    {
                        message.Promise.TrySetResult(null);

                        logger?.LogError("[{Actor}] {Exception}: {Message}\n{StackTrace}", Name, ex.GetType().Name, ex.Message, ex.StackTrace);
                    }
                }
            } while (shutdown == 1 && (Interlocked.CompareExchange(ref processing, 1, 0) != 0));

            gracefulShutdown?.SetResult();
        }
        catch (Exception ex)
        {
            logger?.LogError("[{Actor}] {Exception}: {Message}\n{StackTrace}", Name, ex.GetType().Name, ex.Message, ex.StackTrace);
            
            //Console.Error.WriteLine(ex.Message);
        }
    }
    
    /// <summary>
    /// It retrieves a message from the inbox and invokes the actor by passing one message 
    /// at a time until the pending message list is cleared.
    /// </summary>
    /// <returns></returns>
    private async Task DeliverSingleMessage(ActorMessageReply<TRequest, TResponse> singleMessage)
    {
        try
        {
            if (Actor is null || ActorContext is null || shutdown == 0)
            {
                gracefulShutdown?.SetResult();
                return;
            }

            ActorContext.Runner = this;
            
            if (singleMessage.Sender is not null)
                ActorContext.Sender = singleMessage.Sender;
            else
                ActorContext.Sender = (IGenericActorRef)actorSystem.Nobody;

            ActorContext.Reply = singleMessage;
            ActorContext.ByPassReply = false;

            try
            {
                TResponse? response = await Actor.Receive(singleMessage.Request);

                if (!ActorContext.ByPassReply)
                    singleMessage.Promise.TrySetResult(response);
            }
            catch (Exception ex)
            {
                singleMessage.Promise.TrySetResult(null);

                logger?.LogError("[{Actor}] {Exception}: {Message}\n{StackTrace}", Name, ex.GetType().Name, ex.Message, ex.StackTrace);
            }

            do
            {
                while (inbox.TryDequeue(out ActorMessageReply<TRequest, TResponse>? message))
                {
                    if (shutdown == 0 || ActorContext is null)
                        break;

                    if (message.Sender is not null)
                        ActorContext.Sender = message.Sender;
                    else
                        ActorContext.Sender = (IGenericActorRef)actorSystem.Nobody;

                    ActorContext.Reply = message;
                    ActorContext.ByPassReply = false;

                    try
                    {
                        TResponse? response = await Actor.Receive(message.Request);

                        if (!ActorContext.ByPassReply)
                            message.Promise.TrySetResult(response);
                    }
                    catch (Exception ex)
                    {
                        message.Promise.TrySetResult(null);

                        logger?.LogError("[{Actor}] {Exception}: {Message}\n{StackTrace}", Name, ex.GetType().Name, ex.Message, ex.StackTrace);
                    }
                }
            } while (shutdown == 1 && (Interlocked.CompareExchange(ref processing, 1, 0) != 0));

            gracefulShutdown?.SetResult();
        }
        catch (Exception ex)
        {
            logger?.LogError("[{Actor}] {Exception}: {Message}\n{StackTrace}", Name, ex.GetType().Name, ex.Message, ex.StackTrace);
            
            // Console.Error.WriteLine(ex.Message);
        }
    }

    private async Task DeliverMessageInternal(
        ActorContext<TActor, TRequest, TResponse> actorContext, 
        IActor<TRequest, TResponse> actor,
        ActorMessageReply<TRequest, TResponse> message
    )
    {
        if (message.Sender is not null)
            actorContext.Sender = message.Sender;
        else
            actorContext.Sender = (IGenericActorRef)actorSystem.Nobody;

        actorContext.Reply = message;
        actorContext.ByPassReply = false;

        try
        {
            TResponse? response = await actor.Receive(message.Request);

            if (!actorContext.ByPassReply)
                message.Promise.TrySetResult(response);
        }
        catch (Exception ex)
        {
            message.Promise.TrySetResult(null);

            logger?.LogError("[{Actor}] {Exception}: {Message}\n{StackTrace}", Name, ex.GetType().Name, ex.Message, ex.StackTrace);
        }
    }
}
