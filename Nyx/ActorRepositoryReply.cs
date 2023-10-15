
using System.Collections.Concurrent;

namespace Nyx;

/// <summary>
/// This class maintains an inventory of created actors of a specific type, as well as references to the runners that contain the mailboxes.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ActorRepository<TActor, TRequest, TResponse> : IActorRepositoryRunnable where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    private readonly ActorSystem actorSystem;

    private readonly ConcurrentDictionary<string, (ActorRunner<TActor, TRequest, TResponse>, ActorRef<TActor, TRequest, TResponse>)> actors = new();

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
        foreach (KeyValuePair<string, (ActorRunner<TActor, TRequest, TResponse> runner, ActorRef<TActor, TRequest, TResponse> actorRef)> actor in actors)
        {
            if (!actor.Value.runner.Inbox.IsEmpty)
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
        foreach (KeyValuePair<string, (ActorRunner<TActor, TRequest, TResponse> runner, ActorRef<TActor, TRequest, TResponse> actorRef)> actor in actors)
        {
            if (actor.Value.runner.Processing)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Creates a new actor and returns a reference to it
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
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

        actors.TryAdd(name, (runner, actorRef));        

        return actorRef;
    }

    /// <summary>
    /// Returns a reference to an existing actor or null if it doesn't exist
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest, TResponse>? Get(string name)
    {
        if (actors.TryGetValue(name, out (ActorRunner<TActor, TRequest, TResponse> runner, ActorRef<TActor, TRequest, TResponse> actorRef) actorRef))
            return actorRef.actorRef;

        return null;
    }
}