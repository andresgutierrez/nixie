
using System.Collections.Concurrent;

namespace Nixie;

/// <summary>
/// Schedules messages to be sent to actors at a specified interval.
/// </summary>
public class ActorScheduler : IDisposable
{
    private readonly ConcurrentDictionary<string, Lazy<Timer>> timers = new();    

    /// <summary>
    /// Starts a periodic timer
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="name"></param>
    /// <param name="actorRef"></param>
    /// <param name="request"></param>
    /// <param name="initialDelay"></param>
    /// <param name="interval"></param>
    public Timer StartPeriodicTimer<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, string name, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest> where TRequest : class
    {
        if (timers.ContainsKey(name))
            throw new NixieException("There is already an active timer with this name.");

        Lazy<Timer> timer = timers.GetOrAdd(name, (string name) => new Lazy<Timer>(() => AddPeriodicTimerInternal(actorRef, request, initialDelay, interval)));
        return timer.Value;
    }

    public Timer StartPeriodicTimer<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, string name, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        if (timers.ContainsKey(name))
            throw new NixieException("There is already an active timer with this name.");

        Lazy<Timer> timer = timers.GetOrAdd(name, (string name) => new Lazy<Timer>(() => AddPeriodicTimerInternal(actorRef, request, initialDelay, interval)));
        return timer.Value;
    }

    /// <summary>
    /// Stops a periodic timer
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="NixieException"></exception>
    public void StopPeriodicTimer(string name)
    {
        if (!timers.ContainsKey(name))
            throw new NixieException("There is no timer with this name.");

        if (timers.TryRemove(name, out Lazy<Timer>? timer))
        {
            if (timer.IsValueCreated)
                timer.Value.Dispose();
        }
    }

    private Timer AddPeriodicTimerInternal<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest> where TRequest : class
    {
        return new((x) => SendPeriodicTimer(actorRef, request), null, initialDelay, interval);        
    }
    
    private void SendPeriodicTimer<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request)
        where TActor : IActor<TRequest> where TRequest : class
    {
        actorRef.Send(request);
    }

    private Timer AddPeriodicTimerInternal<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        return new((x) => SendPeriodicTimer(actorRef, request), null, initialDelay, interval);
    }

    private void SendPeriodicTimer<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        actorRef.Send(request);
    }

    public void Dispose()
    {
        foreach (KeyValuePair<string, Lazy<Timer>> timer in timers)
        {
            if (timer.Value.IsValueCreated)
                timer.Value.Value.Dispose();
        }
    }
}
