
using System.Collections.Concurrent;

namespace Nyx;

public interface IActorRepositoryRunnable
{
    public Task Run();
}

public class ActorContext<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public bool Processing { get; set; }

    public ActorRef<TActor, TRequest, TResponse> Actor {get;set;}

    public ActorContext(ActorRef<TActor, TRequest, TResponse> actor)
    {
        Actor = actor;
    }

}

public class ActorRepository<TActor, TRequest, TResponse> : IActorRepositoryRunnable where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    private readonly ActorSystem actorSystem;

    public ConcurrentDictionary<string, ActorContext<TActor, TRequest, TResponse>> actors = new();

    public ActorRepository(ActorSystem actorSystem)
    {
        this.actorSystem = actorSystem;
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

        ActorRef<TActor, TRequest, TResponse>? instance = (ActorRef<TActor, TRequest, TResponse>?)Activator.CreateInstance(typeof(ActorRef<TActor, TRequest, TResponse>), this);

        if (instance is null)
            throw new Exception("Invalid props");

        actors.TryAdd(name, new ActorContext<TActor, TRequest, TResponse>(instance));

        return instance;
    }

    public async Task Run()
    {
        foreach (KeyValuePair<string, ActorContext<TActor, TRequest, TResponse>> actor in actors)
        {
            if (actor.Value.Processing)
                continue;

            actor.Value.Processing = true;

            await actor.Value.Actor.Receive(x);
        }
    }
}

public class ActorSystem
{
    private readonly ConcurrentDictionary<Type, IActorRepositoryRunnable> repositories = new();

    public IActorRef<TActor, TRequest, TResponse> Create<TActor, TRequest, TResponse>(string? name = null) where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        ActorRepository<TActor, TRequest, TResponse> repository = GetRepository<TActor, TRequest, TResponse>();

        return repository.Create(name);
    }

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

    public async Task Run()
    {
        foreach (KeyValuePair<Type, IActorRepositoryRunnable> keyValuePair in repositories)        
            await keyValuePair.Value.Run();
    }
}

public class ActorRef<TActor, TRequest, TResponse> : IActorRef<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public void Send(TRequest message)
    {

    }
}

public interface IActorRef<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public void Send(TRequest message);
}

public interface IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public TResponse Receive(TRequest message);
}


