
using System.Collections.Concurrent;

namespace Nixie;

/// <summary>
/// Passes a message to the active actor reference making sure only one message is processed at a time.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public sealed class ActorRunner<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    private int processing = 1;

    private int shutdown = 1;

    /// <summary>
    /// Returns the name of the actor
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The actor's inbox
    /// </summary>
    public ConcurrentQueue<TRequest> Inbox { get; } = new();

    /// <summary>
    /// Reference to the actual actor
    /// </summary>
    public IActor<TRequest>? Actor { get; set; }

    public bool IsProcessing => processing == 0;

    public bool IsShutdown => shutdown == 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name"></param>
    public ActorRunner(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Enqueues a message to the actor and tries to deliver it.
    /// </summary>
    /// <param name="message"></param>
    public void SendAndTryDeliver(TRequest message)
    {
        if (shutdown == 0)
            return;

        Inbox.Enqueue(message);

        if (1 == Interlocked.Exchange(ref processing, 0))
            _ = Task.Run(DeliverMessages);
    }

    /// <summary>
    /// Try to shutdown the actor and returns a bool indicating success
    /// </summary>
    /// <returns></returns>
    public bool Shutdown()
    {
        return 1 == Interlocked.Exchange(ref shutdown, 0);
    }

    private async Task DeliverMessages()
    {
        if (Actor is null || shutdown == 0)
            return;

        do
        {            
            while (Inbox.TryDequeue(out TRequest? message))
            {
                if (shutdown == 0)
                    break;

                try
                {
                    await Actor.Receive(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
        } while (shutdown == 1 && Interlocked.CompareExchange(ref processing, 1, 0) != 0);
    }
}
