
using System.Collections.Concurrent;

namespace Nixie;

/// <summary>
/// Passes a message to the active actor reference making sure only one message is processed at a time.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ActorRunner<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    /// <summary>
    /// The name/id of the actor.
    /// </summary>
    public string Name { get; }

    private int processing = 1;

    /// <summary>
    /// The inbox of the actor.
    /// </summary>
    public ConcurrentQueue<ActorMessageReply<TRequest, TResponse>> Inbox { get; } = new();

    /// <summary>
    /// The reference to the actor.
    /// </summary>
    public IActor<TRequest, TResponse>? Actor { get; set; }

    /// <summary>
    /// True if the actor is processing a message.
    /// </summary>
    public bool Processing => processing == 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name"></param>
    /// <param name="actor"></param>
    public ActorRunner(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Enqueues a message to the actor and tries to deliver it.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public ActorMessageReply<TRequest, TResponse> SendAndTryDeliver(TRequest message)
    {
        ActorMessageReply<TRequest, TResponse> messageReply = new(message);

        Inbox.Enqueue(messageReply);

        if (1 == Interlocked.Exchange(ref processing, 0))
            _ = Task.Run(DeliverMessages).ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception is not null)
                    throw t.Exception;
            });

        return messageReply;
    }

    private async Task DeliverMessages()
    {
        if (Actor is null)
            return;

        do
        {
            while (Inbox.TryDequeue(out ActorMessageReply<TRequest, TResponse>? messageReply))
            {
                try
                {
                    messageReply.SetCompleted(await Actor.Receive(messageReply.Request));
                }
                catch (Exception ex)
                {
                    messageReply.SetCompleted(null);

                    Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
        } while (Interlocked.CompareExchange(ref processing, 1, 0) != 0);        
    }
}
