
using Microsoft.Extensions.Logging;
using Nixie.Actors;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Nixie;

/// <summary>
/// The actor system encapsulates and encompasses all the actors and their references created. It is possible to have many independent actor systems running.
/// </summary>
public sealed class ActorSystem : IDisposable
{
    private readonly ActorScheduler scheduler;

    private readonly IServiceProvider? serviceProvider;

    private readonly ILogger? logger;

    private readonly ConcurrentDictionary<Type, Lazy<IActorRepositoryRunnable>> repositories = new();

    private readonly IActorRef<NobodyActor, object> nobody;

    /// <summary>
    /// Returns the reference to the nobody actor. This actor is used when a message is sent to an actor that doesn't exist.
    /// </summary>
    public IActorRef<NobodyActor, object> Nobody => nobody;

    /// <summary>
    /// Returns the actor scheduler
    /// </summary>
    public ActorScheduler Scheduler => scheduler;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="logger"></param>
    public ActorSystem(IServiceProvider? serviceProvider = null, ILogger? logger = null)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.scheduler = new(logger);

        nobody = Spawn<NobodyActor, object>();
    }

    /// <summary>
    /// Creates a new request/response actor and returns a typed reference.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest, TResponse> Spawn<TActor, TRequest, TResponse>(string? name = null, params object[]? args)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        ActorRepository<TActor, TRequest, TResponse> repository = GetRepository<TActor, TRequest, TResponse>();

        return repository.Spawn(name, args);
    }

    /// <summary>
    /// Creates a new fire-n-forget actor and returns a typed reference.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest> Spawn<TActor, TRequest>(string? name = null, params object[]? args)
        where TActor : IActor<TRequest> where TRequest : class
    {
        ActorRepository<TActor, TRequest> repository = GetRepository<TActor, TRequest>();

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
    public IActorRef<TActor, TRequest, TResponse>? Get<TActor, TRequest, TResponse>(string name)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        ActorRepository<TActor, TRequest, TResponse> repository = GetRepository<TActor, TRequest, TResponse>();

        return repository.Get(name);
    }

    /// <summary>
    /// Returns a fire-n-forget actor by its name and null if it doesn't exist.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public IActorRef<TActor, TRequest>? Get<TActor, TRequest>(string name) where TActor : IActor<TRequest>
        where TRequest : class
    {
        ActorRepository<TActor, TRequest> repository = GetRepository<TActor, TRequest>();

        return repository.Get(name);
    }

    /// <summary>
    /// Shutdowns an actor by its name
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Shutdown<TActor, TRequest, TResponse>(string name) where TActor : IActor<TRequest, TResponse>
        where TRequest : class where TResponse : class
    {
        ActorRepository<TActor, TRequest, TResponse> repository = GetRepository<TActor, TRequest, TResponse>();

        return repository.Shutdown(name);
    }

    /// <summary>
    /// Shutdowns an actor by its reference
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Shutdown<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        ActorRepository<TActor, TRequest, TResponse> repository = GetRepository<TActor, TRequest, TResponse>();

        return repository.Shutdown(actorRef);
    }

    /// <summary>
    /// Shutdowns an actor by its name
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Shutdown<TActor, TRequest>(string name) where TActor : IActor<TRequest>
        where TRequest : class
    {
        ActorRepository<TActor, TRequest> repository = GetRepository<TActor, TRequest>();

        return repository.Shutdown(name);
    }

    /// <summary>
    /// Shutdowns an actor by its reference
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Shutdown<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef)
        where TActor : IActor<TRequest> where TRequest : class
    {
        ActorRepository<TActor, TRequest> repository = GetRepository<TActor, TRequest>();

        return repository.Shutdown(actorRef);
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
        Lazy<IActorRepositoryRunnable> repository = repositories.GetOrAdd(
            typeof(TActor),
            (type) => new Lazy<IActorRepositoryRunnable>(() => CreateRepository<TActor, TRequest, TResponse>())
        );

        return (ActorRepository<TActor, TRequest, TResponse>)repository.Value;
    }

    private ActorRepository<TActor, TRequest, TResponse> CreateRepository<TActor, TRequest, TResponse>()
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        ActorRepository<TActor, TRequest, TResponse> repository = new(this, serviceProvider, logger);
        return repository;
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
        Lazy<IActorRepositoryRunnable> repository = repositories.GetOrAdd(
            typeof(TActor),
            (type) => new Lazy<IActorRepositoryRunnable>(() => CreateRepository<TActor, TRequest>())
        );

        return (ActorRepository<TActor, TRequest>)repository.Value;
    }

    private ActorRepository<TActor, TRequest> CreateRepository<TActor, TRequest>()
        where TActor : IActor<TRequest> where TRequest : class
    {
        ActorRepository<TActor, TRequest> repository = new(this, serviceProvider, logger);
        return repository;
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
    /// Stops all timers running or scheduled for the specified actor.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="actorRef"></param>
    public void StopAllTimers<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        scheduler.StopAllTimers(actorRef);
    }

    /// <summary>
    /// Stops all timers running or scheduled for the specified actor.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="actorRef"></param>
    public void StopAllTimers<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef)
        where TActor : IActor<TRequest> where TRequest : class
    {
        scheduler.StopAllTimers(actorRef);
    }

    /// <summary>
    /// Waits for all the actors in the system to finish processing their messages.
    /// </summary>
    /// <returns></returns>
    public async Task Wait()
    {
        Stopwatch stopWatch = new();
        string? pendingActorName = null, processingName = null;

        while (true)
        {
            bool completed = true;

            foreach (KeyValuePair<Type, Lazy<IActorRepositoryRunnable>> repository in repositories)
            {
                Lazy<IActorRepositoryRunnable> lazyRepository = repository.Value;

                if (!lazyRepository.IsValueCreated)
                    continue;

                if (lazyRepository.Value.HasPendingMessages(out pendingActorName) || lazyRepository.Value.IsProcessing(out processingName))
                {
                    await Task.Yield();
                    completed = false;
                    break;
                }
            }

            if (completed)
                break;

            if (stopWatch.ElapsedMilliseconds > 10000)
            {
                logger?.LogWarning("Timeout waiting for actor {PendingActorName}/{ProcessingName}", pendingActorName, processingName);
                break;
            }
        }
    }

    public void Dispose()
    {
        scheduler.Dispose();
    }
}
