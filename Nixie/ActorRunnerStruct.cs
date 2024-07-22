
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// Passes a struct message to the active actor reference making sure only one message is processed at a time.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public sealed class ActorRunnerStruct<TActor, TRequest> where TActor : IActorStruct<TRequest> where TRequest : struct
{
    private readonly ActorSystem actorSystem;

    private readonly ILogger? logger;

    private readonly ConcurrentQueue<ActorMessage<TRequest>> inbox = new();

    private int processing = 1;

    private int shutdown = 1;

    /// <summary>
    /// Returns the name of the actor
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
    /// Reference to the actual actor
    /// </summary>
    public IActorStruct<TRequest>? Actor { get; set; }

    /// <summary>
    /// Reference to the current actor context
    /// </summary>
    public ActorContextStruct<TActor, TRequest>? ActorContext { get; set; }

    /// <summary>
    /// Returns true if the runner is processing messages
    /// </summary>
    public bool IsProcessing => processing == 0;

    /// <summary>
    /// Returns true if the actor is shutdown
    /// </summary>
    public bool IsShutdown => shutdown == 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="actorSystem"></param>
    /// <param name="logger"></param>
    /// <param name="name"></param>
    public ActorRunnerStruct(ActorSystem actorSystem, ILogger? logger, string name)
    {
        this.actorSystem = actorSystem;
        this.logger = logger;

        Name = name;
    }

    /// <summary>
    /// Enqueues a message to the actor and tries to deliver it.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    public void SendAndTryDeliver(TRequest message, IGenericActorRef? sender)
    {
        if (shutdown == 0)
            return;

        inbox.Enqueue(new ActorMessage<TRequest>(message, sender));

        if (1 == Interlocked.Exchange(ref processing, 0))
            _ = DeliverMessages();
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
    /// Enqueues a message to the actor and tries to deliver it.
    /// The request/response type actors use an object to assign the response once completed.    
    /// </summary>
    /// <returns></returns>
    private async Task DeliverMessages()
    {
        try
        {
            if (Actor is null || ActorContext is null || shutdown == 0)
                return;

            ActorContext.Runner = this;

            do
            {
                while (inbox.TryDequeue(out ActorMessage<TRequest> message))
                {
                    if (shutdown == 0)
                        break;

                    if (ActorContext is not null)
                    {
                        if (message.Sender is not null)
                            ActorContext.Sender = message.Sender;
                        else
                            ActorContext.Sender = (IGenericActorRef)actorSystem.Nobody;
                    }

                    try
                    {
                        await Actor.Receive(message.Request);
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError("[{Actor}] {Exception}: {Message}\n{StackTrace}", Name, ex.GetType().Name, ex.Message, ex.StackTrace);
                    }
                }
            } while (shutdown == 1 && Interlocked.CompareExchange(ref processing, 1, 0) != 0);
        }
        catch (Exception ex)
        {
            logger?.LogError("[{Actor}] {Exception}: {Message}\n{StackTrace}", Name, ex.GetType().Name, ex.Message, ex.StackTrace);
        }
    }
}
