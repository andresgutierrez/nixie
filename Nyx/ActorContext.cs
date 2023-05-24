
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
}
