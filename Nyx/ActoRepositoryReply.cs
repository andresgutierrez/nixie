
using System.Collections.Concurrent;

namespace Nyx;

public sealed class ActorRepository<TActor, TRequest, TResponse> : IActorRepositoryRunnable where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    private readonly ActorSystem actorSystem;

    private readonly ConcurrentDictionary<string, ActorRunner<TActor, TRequest, TResponse>> actors = new();

    public ActorRepository(ActorSystem actorSystem)
    {
        this.actorSystem = actorSystem;
    }

    public bool HasPendingMessages()
    {
        foreach (KeyValuePair<string, ActorRunner<TActor, TRequest, TResponse>> context in actors)
        {
            if (!context.Value.Inbox.IsEmpty)
                return true;
        }
        return false;
    }

    public bool IsProcessing()
    {
        foreach (KeyValuePair<string, ActorRunner<TActor, TRequest, TResponse>> context in actors)
        {
            if (context.Value.Processing)
                return true;
        }
        return false;
    }

    public IActorRef<TActor, TRequest, TResponse> Create(string? name = null)
    {
        if (!string.IsNullOrEmpty(name))
        {
            name = name.ToLowerInvariant();

            if (actors.ContainsKey(name))
                throw new Exception("Actor already exists");
        }
        else
        {
            name = Guid.NewGuid().ToString();
        }

        ActorContext actorContext = new(actorSystem);

        TActor? actor = (TActor?)Activator.CreateInstance(typeof(TActor), actorContext);
        if (actor is null)
            throw new Exception("Invalid actor");

        ActorRunner<TActor, TRequest, TResponse> runner = new(name, actor);

        ActorRef<TActor, TRequest, TResponse>? actorRef = (ActorRef<TActor, TRequest, TResponse>?)Activator.CreateInstance(typeof(ActorRef<TActor, TRequest, TResponse>), runner);

        if (actorRef is null)
            throw new Exception("Invalid props");

        actors.TryAdd(name, runner);

        return actorRef;
    }
}