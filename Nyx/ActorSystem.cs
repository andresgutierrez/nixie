
using System.Collections.Concurrent;

namespace Nyx;

/// <summary>
/// The actor system encapsulates and encompasses all the actors and their references created. It is possible to have many independent actor systems running.
/// </summary>
public sealed class ActorSystem
{
    private readonly ConcurrentDictionary<Type, IActorRepositoryRunnable> repositories = new();

    /// <summary>
    /// Creates a new request/response actor and returns a typed reference.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest, TResponse> Create<TActor, TRequest, TResponse>(string? name = null) where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        ActorRepository<TActor, TRequest, TResponse> repository = GetRepository<TActor, TRequest, TResponse>();

        return repository.Create(name);
    }

    /// <summary>
    /// Returns a request/response actor by its name and null if it doesn't exist.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest, TResponse>? Get<TActor, TRequest, TResponse>(string name) where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        ActorRepository<TActor, TRequest, TResponse> repository = GetRepository<TActor, TRequest, TResponse>();

        return repository.Get(name);
    }

    /// <summary>
    /// Creates a new fire-n-forget actor and returns a typed reference.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest> Create<TActor, TRequest>(string? name = null) where TActor : IActor<TRequest> where TRequest : class
    {
        ActorRepository<TActor, TRequest> repository = GetRepository<TActor, TRequest>();

        return repository.Create(name);
    }

    /// <summary>
    /// Returns a fire-n-forget actor by its name and null if it doesn't exist.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest>? Get<TActor, TRequest>(string name) where TActor : IActor<TRequest> where TRequest : class
    {
        ActorRepository<TActor, TRequest> repository = GetRepository<TActor, TRequest>();

        return repository.Get(name);
    }

    /// <summary>
    /// Returns the repository where the current references to request/response actors are stored.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public ActorRepository<TActor, TRequest, TResponse> GetRepository<TActor, TRequest, TResponse>() where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        if (!repositories.TryGetValue(typeof(TActor), out IActorRepositoryRunnable? unitOfWorker))
        {
            ActorRepository<TActor, TRequest, TResponse> newUnitOfWork = new(this);
            repositories.TryAdd(typeof(TActor), newUnitOfWork);
            return newUnitOfWork;
        }

        return (ActorRepository<TActor, TRequest, TResponse>)unitOfWorker;
    }

    /// <summary>
    /// Returns the repository where the current references to fire-n-forget actors are stored.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns></returns>
    public ActorRepository<TActor, TRequest> GetRepository<TActor, TRequest>() where TActor : IActor<TRequest> where TRequest : class
    {
        if (!repositories.TryGetValue(typeof(TActor), out IActorRepositoryRunnable? unitOfWorker))
        {
            ActorRepository<TActor, TRequest> newUnitOfWork = new(this);
            repositories.TryAdd(typeof(TActor), newUnitOfWork);
            return newUnitOfWork;
        }

        return (ActorRepository<TActor, TRequest>)unitOfWorker;
    }

    public async Task Wait()
    {
        while (true)
        {
            bool completed = true;

            foreach (KeyValuePair<Type, IActorRepositoryRunnable> x in repositories)
            {
                //Console.WriteLine("{0} HP={1} IsP={2}", x.Key, x.Value.HasPendingMessages(), x.Value.IsProcessing());

                if (x.Value.HasPendingMessages() || x.Value.IsProcessing())
                {
                    await Task.Yield();
                    completed = false;
                    break;
                }
            }

            if (completed)
                break;
        }
    }
}
