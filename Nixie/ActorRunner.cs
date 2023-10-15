
using System.Collections.Concurrent;

namespace Nixie;

public sealed class ActorRunner<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    public string Name { get; }

    private int processing = 1;

    public ConcurrentQueue<TRequest> Inbox { get; } = new();

    public IActor<TRequest>? Actor { get; set; }

    public bool Processing => processing == 0;

    public ActorRunner(string name)
    {
        Name = name;
    }

    public void SendAndTryDeliver(TRequest message)
    {
        Inbox.Enqueue(message);

        if (1 == Interlocked.Exchange(ref processing, 0))
            _ = Task.Run(DeliverMessages);
    }

    private async Task DeliverMessages()
    {
        if (Actor is null)
            return;

        do
        {
            while (Inbox.TryDequeue(out TRequest? message))
            {
                try
                {
                    await Actor.Receive(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
        } while (Interlocked.CompareExchange(ref processing, 1, 0) != 0);
    }
}
