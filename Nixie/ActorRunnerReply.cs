
using System.Collections.Concurrent;
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

    private int processing = 1;

    private int shutdown = 1;

    /// <summary>
    /// The name/id of the actor.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The inbox of the actor.
    /// </summary>
    public ConcurrentQueue<ActorMessageReply<TRequest, TResponse>> Inbox { get; } = new();

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
    /// The request/response type actors use an object to assign the response once completed. The "Ask" operations spin until they receive the response.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    /// <param name="parentReply"></param>
    /// <returns></returns>
    public TaskCompletionSource<TResponse?> SendAndTryDeliver(TRequest message, IGenericActorRef? sender, ActorMessageReply<TRequest, TResponse>? parentReply)
    {
        ActorMessageReply<TRequest, TResponse> messageReply;

        TaskCompletionSource<TResponse?> promise = new();

        if (parentReply is not null)
            messageReply = parentReply;
        else
            messageReply = new(message, sender, promise);        

        if (shutdown == 0)
        {
            promise.SetCanceled();
            return promise;
        }

        Inbox.Enqueue(messageReply);

        if (1 == Interlocked.Exchange(ref processing, 0))
            _ = DeliverMessages();

        return promise;
    }

    /// <summary>
    /// Try to shutdown the actor and returns a bool indicating success
    /// </summary>
    /// <returns></returns>
    public bool Shutdown()
    {
        return 1 == Interlocked.Exchange(ref shutdown, 0);
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
                return;

            do
            {
                while (Inbox.TryDequeue(out ActorMessageReply<TRequest, TResponse>? message))
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
                            message.Promise.SetResult(response);
                    }
                    catch (Exception ex)
                    {
                        message.Promise.SetResult(null);

                        logger?.LogError("[{Actor}] {Exception}: {Message}\n{StackTrace}", Name, ex.GetType().Name, ex.Message, ex.StackTrace);
                    }
                }
            } while (shutdown == 1 && (Interlocked.CompareExchange(ref processing, 1, 0) != 0));
        }
        catch (Exception ex)
        {
            logger?.LogError("[{Actor}] {Exception}: {Message}\n{StackTrace}", Name, ex.GetType().Name, ex.Message, ex.StackTrace);
        }
    }
}
