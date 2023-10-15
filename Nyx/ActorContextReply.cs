
using System.Collections.Concurrent;

namespace Nyx;

public sealed class ActorContext<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public string Name { get; }

    private int processing = 1;

    public ConcurrentQueue<ActorMessageReply<TRequest, TResponse>> Inbox { get; } = new();

    public IActor<TRequest, TResponse> Actor { get; }

    public bool Processing => processing == 0;

    public ActorContext(string name, IActor<TRequest, TResponse> actor)
    {
        Name = name;
        Actor = actor;
    }

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
