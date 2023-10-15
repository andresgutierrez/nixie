
using System.Collections.Concurrent;

namespace Nixie;

/// <summary>
/// This class maintains an inventory of created actors of a specific type, as well as references to the runners that contain the mailboxes.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ActorRepository<TActor, TRequest, TResponse> : IActorRepositoryRunnable where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    private readonly ActorSystem actorSystem;

    private readonly ConcurrentDictionary<string, Lazy<(ActorRunner<TActor, TRequest, TResponse>, ActorRef<TActor, TRequest, TResponse>)>> actors = new();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="actorSystem"></param>
    public ActorRepository(ActorSystem actorSystem)
    {
        this.actorSystem = actorSystem;
    }

    /// <summary>
    /// Check if any of the actors have pending messages
    /// </summary>
    /// <returns></returns>
    public bool HasPendingMessages()
    {
        foreach (KeyValuePair<string, Lazy<(ActorRunner<TActor, TRequest, TResponse> runner, ActorRef<TActor, TRequest, TResponse> actorRef)>> actor in actors)
        {
            Lazy<(ActorRunner<TActor, TRequest, TResponse> runner, ActorRef<TActor, TRequest, TResponse> actorRef)> lazyValue = actor.Value;

            if (lazyValue.IsValueCreated && !lazyValue.Value.runner.Inbox.IsEmpty)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Check if any of the actors are processing messages
    /// </summary>
    /// <returns></returns>
    public bool IsProcessing()
    {
        foreach (KeyValuePair<string, Lazy<(ActorRunner<TActor, TRequest, TResponse> runner, ActorRef<TActor, TRequest, TResponse> actorRef)>> actor in actors)
        {
            Lazy<(ActorRunner<TActor, TRequest, TResponse> runner, ActorRef<TActor, TRequest, TResponse> actorRef)> lazyValue = actor.Value;

            if (lazyValue.IsValueCreated && lazyValue.Value.runner.Processing)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Creates a new actor and returns a reference to it
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="NixieException"></exception>
    public IActorRef<TActor, TRequest, TResponse> Spawn(string? name = null)
    {
        if (!string.IsNullOrEmpty(name))
        {
            name = name.ToLowerInvariant();

            if (actors.ContainsKey(name))
                throw new NixieException("Actor already exists");
        }
        else
        {
            name = Guid.NewGuid().ToString();
        }

        Lazy<(ActorRunner<TActor, TRequest, TResponse> runner, ActorRef<TActor, TRequest, TResponse> actorRef)> actor = actors.GetOrAdd(
            name,
            (string name) => new Lazy<(ActorRunner<TActor, TRequest, TResponse>, ActorRef<TActor, TRequest, TResponse>)>(() => CreateInternal(name))
        );

        return actor.Value.actorRef;
    }

    private (ActorRunner<TActor, TRequest, TResponse>, ActorRef<TActor, TRequest, TResponse>) CreateInternal(string name)
    {
        ActorRunner<TActor, TRequest, TResponse> runner = new(name);

        ActorRef<TActor, TRequest, TResponse>? actorRef = (ActorRef<TActor, TRequest, TResponse>?)Activator.CreateInstance(typeof(ActorRef<TActor, TRequest, TResponse>), runner);

        if (actorRef is null)
            throw new NixieException("Invalid props");

        ActorContext<TActor, TRequest, TResponse> actorContext = new(actorSystem, actorRef);

        TActor? actor = (TActor?)Activator.CreateInstance(typeof(TActor), actorContext);
        if (actor is null)
            throw new NixieException("Invalid actor");

        runner.Actor = actor;

        return (runner, actorRef);
    }

    /// <summary>
    /// Returns a reference to an existing actor
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest, TResponse>? Get(string name)
    {
        name = name.ToLowerInvariant();

        if (actors.TryGetValue(name, out Lazy<(ActorRunner<TActor, TRequest, TResponse> runner, ActorRef<TActor, TRequest, TResponse> actorRef)>? actor))
            return actor.Value.actorRef;

        return null;
    }
}
