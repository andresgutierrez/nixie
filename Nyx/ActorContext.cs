
using System.Collections.Concurrent;

namespace Nyx;

public sealed class ActorContext<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public bool Processing { get; set; }

    public ConcurrentQueue<TRequest> Inbox { get; } = new();

    public IActor<TRequest, TResponse> Actor { get; }

    public ActorContext(IActor<TRequest, TResponse> actor)
    {
        Actor = actor;
    }

    public void SendAndTryRun(TRequest message)
    {
        Inbox.Enqueue(message);

        if (!Processing)
            _ = Task.Run(RunActor);
    }

    private async Task RunActor()
    {
        if (Processing)
            return;

        Processing = true;

        while (Inbox.TryDequeue(out TRequest? message))
            await Actor.Receive(message);

        Processing = false;
    }
}

public sealed class ActorContext<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    public string Name { get; }

    private int processing = 1;

    public ConcurrentQueue<TRequest> Inbox { get; } = new();

    public IActor<TRequest> Actor { get; }

    public bool Processing => processing == 1;

    public ActorContext(string name, IActor<TRequest> actor)
    {
        Name = name;
        Actor = actor;
    }

    public void SendAndTryRun(TRequest message)
    {
        Inbox.Enqueue(message);

        if (1 == Interlocked.Exchange(ref processing, 0))
            _ = Task.Run(RunActor);
    }

    private async Task RunActor()
    {
        while (Inbox.TryDequeue(out TRequest? message))
            await Actor.Receive(message);

        Interlocked.Exchange(ref processing, 1);
    }
}
