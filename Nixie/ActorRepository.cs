
using System.Collections.Concurrent;

namespace Nixie;

/// <summary>
/// This class maintains an inventory of created actors of a specific type, as well as references to the runners that contain the mailboxes.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ActorRepository<TActor, TRequest> : IActorRepositoryRunnable where TActor : IActor<TRequest> where TRequest : class
{
    private readonly ActorSystem actorSystem;
    
    private readonly ConcurrentDictionary<string, (ActorRunner<TActor, TRequest> , ActorRef<TActor, TRequest>)> actors = new();

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
        foreach (KeyValuePair<string, (ActorRunner<TActor, TRequest> runner, ActorRef<TActor, TRequest> actorRef)> actor in actors)
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
        foreach (KeyValuePair<string, (ActorRunner<TActor, TRequest> runner, ActorRef<TActor, TRequest> actorRef)> actor in actors)
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

        ActorContext actorContext = new(actorSystem);

        TActor? actor = (TActor?)Activator.CreateInstance(typeof(TActor), actorContext);
        if (actor is null)
            throw new Exception("Invalid actor");

        ActorRunner<TActor, TRequest> runner = new(name, actor);

        ActorRef<TActor, TRequest>? actorRef = (ActorRef<TActor, TRequest>?)Activator.CreateInstance(typeof(ActorRef<TActor, TRequest>), runner);

        if (actorRef is null)
            throw new Exception("Invalid props");

        actors.TryAdd(name, (runner, actorRef));        

        return actorRef;
    }

    /// <summary>
    /// Returns a reference to an existing actor
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest>? Get(string name)
    {
        if (actors.TryGetValue(name, out (ActorRunner<TActor, TRequest> runner, ActorRef<TActor, TRequest> actorRef) actor))
            return actor.actorRef;

        return null;
    }
}
