
using System.Collections.Concurrent;

namespace Nixie;

/// <summary>
/// The actor system encapsulates and encompasses all the actors and their references created. It is possible to have many independent actor systems running.
/// </summary>
public sealed class ActorSystem : IDisposable
{
    private readonly ActorScheduler scheduler = new();

    private readonly ConcurrentDictionary<Type, IActorRepositoryRunnable> repositories = new();

    /// <summary>
    /// Returns the actor scheduler
    /// </summary>
    public ActorScheduler Scheduler => scheduler;

    /// <summary>
    /// Creates a new request/response actor and returns a typed reference.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest, TResponse> Spawn<TActor, TRequest, TResponse>(string? name = null, params object[]? args) where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        ActorRepository<TActor, TRequest, TResponse> repository = GetRepository<TActor, TRequest, TResponse>();

        return repository.Spawn(name, args);
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
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest> Spawn<TActor, TRequest>(string? name = null, params object[]? args) where TActor : IActor<TRequest> where TRequest : class
    {
        ActorRepository<TActor, TRequest> repository = GetRepository<TActor, TRequest>();

        return repository.Spawn(name, args);
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
    public ActorRepository<TActor, TRequest, TResponse> GetRepository<TActor, TRequest, TResponse>()
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
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
    public ActorRepository<TActor, TRequest> GetRepository<TActor, TRequest>()
        where TActor : IActor<TRequest> where TRequest : class
    {
        if (!repositories.TryGetValue(typeof(TActor), out IActorRepositoryRunnable? unitOfWorker))
        {
            ActorRepository<TActor, TRequest> newUnitOfWork = new(this);
            repositories.TryAdd(typeof(TActor), newUnitOfWork);
            return newUnitOfWork;
        }

        return (ActorRepository<TActor, TRequest>)unitOfWorker;
    }

    /// <summary>
    /// Creates a new periodic timer that will send a message to the specified actor every interval.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="name"></param>
    /// <param name="actorRef"></param>
    /// <param name="request"></param>
    /// <param name="initialDelay"></param>
    /// <param name="interval"></param>
    public void StartPeriodicTimer<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, string name, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest> where TRequest : class
    {
        scheduler.StartPeriodicTimer(actorRef, name, request, initialDelay, interval);
    }

    /// <summary>
    /// Creates a new periodic timer that will send a message to the specified actor every interval.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <param name="actorRef"></param>
    /// <param name="request"></param>
    /// <param name="initialDelay"></param>
    /// <param name="interval"></param>
    public void StartPeriodicTimer<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, string name, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        scheduler.StartPeriodicTimer(actorRef, name, request, initialDelay, interval);
    }

    /// <summary>
    /// Schedules a message to be sent to an actor once after a specified delay.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="actorRef"></param>
    /// <param name="request"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public void ScheduleOnce<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan delay)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        scheduler.ScheduleOnce(actorRef, request, delay);
    }

    /// <summary>
    /// Schedules a message to be sent to an actor once after a specified delay.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="actorRef"></param>
    /// <param name="request"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public void ScheduleOnce<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request, TimeSpan delay)
        where TActor : IActor<TRequest> where TRequest : class
    {
        scheduler.ScheduleOnce(actorRef, request, delay);
    }

    /// <summary>
    /// Stops a periodic timer
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="NixieException"></exception>
    public void StopPeriodicTimer<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, string name)
        where TActor : IActor<TRequest> where TRequest : class
    {
        scheduler.StopPeriodicTimer(actorRef, name);
    }

    /// <summary>
    /// Stops a periodic timer
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="NixieException"></exception>
    public void StopPeriodicTimer<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, string name)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        scheduler.StopPeriodicTimer(actorRef, name);
    }

    /// <summary>
    /// Waits for all the actors in the system to finish processing their messages.
    /// </summary>
    /// <returns></returns>
    public async Task Wait()
    {
        while (true)
        {
            bool completed = true;

            foreach (KeyValuePair<Type, IActorRepositoryRunnable> repository in repositories)
            {
                //Console.WriteLine("{0} HP={1} IsP={2}", x.Key, x.Value.HasPendingMessages(), x.Value.IsProcessing());

                if (repository.Value.HasPendingMessages() || repository.Value.IsProcessing())
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

    public void Dispose()
    {
        scheduler.Dispose();
    }
}
