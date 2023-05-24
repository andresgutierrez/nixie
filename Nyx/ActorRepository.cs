
using System.Collections.Concurrent;

namespace Nyx;

public sealed class ActorRepository<TActor, TRequest, TResponse> : IActorRepositoryRunnable where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    private readonly ActorSystem actorSystem;

    public ConcurrentDictionary<string, ActorContext<TActor, TRequest, TResponse>> actors = new();

    public ActorRepository(ActorSystem actorSystem)
    {
        this.actorSystem = actorSystem;
    }

    public bool HasPendingMessages()
    {
        foreach (KeyValuePair<string, ActorContext<TActor, TRequest, TResponse>> context in actors)
        {
            if (!context.Value.Inbox.IsEmpty)
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

        TActor? actor = (TActor?)Activator.CreateInstance(typeof(TActor));
        if (actor is null)
            throw new Exception("Invalid actor");

        ActorContext<TActor, TRequest, TResponse> context = new(actor);

        ActorRef<TActor, TRequest, TResponse>? actorRef = (ActorRef<TActor, TRequest, TResponse>?)Activator.CreateInstance(typeof(ActorRef<TActor, TRequest, TResponse>), context);

        if (actorRef is null)
            throw new Exception("Invalid props");

        actors.TryAdd(name, context);

        return actorRef;
    }

    public async Task Run()
    {
        foreach (KeyValuePair<string, ActorContext<TActor, TRequest, TResponse>> context in actors)
        {
            if (context.Value.Processing)
                continue;

            if (!context.Value.Inbox.TryDequeue(out TRequest? request))
                continue;

            context.Value.Processing = true;

            await context.Value.Actor.Receive(request);

            context.Value.Processing = false;
        }
    }
}

public sealed class ActorRepository<TActor, TRequest> : IActorRepositoryRunnable where TActor : IActor<TRequest> where TRequest : class
{
    private readonly ActorSystem actorSystem;

    public ConcurrentDictionary<string, ActorContext<TActor, TRequest>> actors = new();

    public ActorRepository(ActorSystem actorSystem)
    {
        this.actorSystem = actorSystem;
    }

    public bool HasPendingMessages()
    {
        foreach (KeyValuePair<string, ActorContext<TActor, TRequest>> context in actors)
        {
            if (!context.Value.Inbox.IsEmpty)
                return true;
        }
        return false;
    }

    public IActorRef<TActor, TRequest> Create(string? name = null)
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

        TActor? actor = (TActor?)Activator.CreateInstance(typeof(TActor));
        if (actor is null)
            throw new Exception("Invalid actor");

        ActorContext<TActor, TRequest> context = new(actor);

        ActorRef<TActor, TRequest>? actorRef = (ActorRef<TActor, TRequest>?)Activator.CreateInstance(typeof(ActorRef<TActor, TRequest>), context);

        if (actorRef is null)
            throw new Exception("Invalid props");

        actors.TryAdd(name, context);

        return actorRef;
    }

    public async Task Run()
    {
        foreach (KeyValuePair<string, ActorContext<TActor, TRequest>> context in actors)
        {
            if (context.Value.Processing)
                continue;

            if (!context.Value.Inbox.TryDequeue(out TRequest? request))
                continue;

            context.Value.Processing = true;

            await context.Value.Actor.Receive(request);

            context.Value.Processing = false;
        }
    }
}
