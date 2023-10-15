
using System.Collections.Concurrent;

namespace Nyx;

public sealed class ActorRunner<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    public string Name { get; }

    private int processing = 1;

    public ConcurrentQueue<TRequest> Inbox { get; } = new();

    public IActor<TRequest> Actor { get; }

    public bool Processing => processing == 0;

    public ActorRunner(string name, IActor<TRequest> actor)
    {
        Name = name;
        Actor = actor;
    }

    public void SendAndTryDeliver(TRequest message)
    {
        Inbox.Enqueue(message);

        if (1 == Interlocked.Exchange(ref processing, 0))
            _ = Task.Run(DeliverMessages);
    }

    private async Task DeliverMessages()
    {
        do
        {
            while (Inbox.TryDequeue(out TRequest? message))
            {
                //Console.WriteLine("Dequeued one {0}", Name);

                try
                {
                    await Actor.Receive(message);

                    //Console.WriteLine("Completed one {0}", Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
        } while (Interlocked.CompareExchange(ref processing, 1, 0) != 0);

        //Console.WriteLine("Finished processing {0} {1} {2}", Name, Inbox.Count, processing);
    }
}
