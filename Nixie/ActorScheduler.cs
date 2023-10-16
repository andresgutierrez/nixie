
using System.Collections.Concurrent;

namespace Nixie;

/// <summary>
/// Schedules messages to be sent to actors at a specified interval.
/// </summary>
public class ActorScheduler : IDisposable
{
    private readonly ConcurrentDictionary<object, Lazy<ConcurrentBag<Timer>>> onceTimers = new();

    private readonly ConcurrentDictionary<object, Lazy<ConcurrentDictionary<string, Lazy<Timer>>>> periodicTimers = new();

    /// <summary>
    /// Schedules a message to be sent to an actor once after a specified delay at a specified interval.
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
        if (periodicTimers.ContainsKey(name))
            throw new NixieException("There is already an active timer with this name.");

        Lazy<ConcurrentDictionary<string, Lazy<Timer>>> timers = periodicTimers.GetOrAdd(actorRef, (actorRef) => new Lazy<ConcurrentDictionary<string, Lazy<Timer>>>());
        Lazy<Timer> timer = timers.Value.GetOrAdd(name, (string name) => new Lazy<Timer>(() => AddPeriodicTimerInternal(actorRef, request, initialDelay, interval)));
        return timer.Value;
    }

    /// <summary>
    /// Schedules a message to be sent to an actor once after a specified delay at a specified interval.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="actorRef"></param>
    /// <param name="name"></param>
    /// <param name="request"></param>
    /// <param name="initialDelay"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    /// <exception cref="NixieException"></exception>
    public Timer StartPeriodicTimer<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, string name, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        if (periodicTimers.ContainsKey(name))
            throw new NixieException("There is already an active timer with this name.");

        Lazy<ConcurrentDictionary<string, Lazy<Timer>>> timers = periodicTimers.GetOrAdd(actorRef, (actorRef) => new Lazy<ConcurrentDictionary<string, Lazy<Timer>>>());
        Lazy<Timer> timer = timers.Value.GetOrAdd(name, (string name) => new Lazy<Timer>(() => AddPeriodicTimerInternal(actorRef, request, initialDelay, interval)));
        return timer.Value;
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
    public ConcurrentBag<Timer> ScheduleOnce<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan delay)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        Lazy<ConcurrentBag<Timer>> timers = onceTimers.GetOrAdd(actorRef, (object actorRef) => new Lazy<ConcurrentBag<Timer>>());
        timers.Value.Add(ScheduleOnceTimer(actorRef, request, delay));
        return timers.Value;
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
    public ConcurrentBag<Timer> ScheduleOnce<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request, TimeSpan delay)
        where TActor : IActor<TRequest> where TRequest : class
    {
        Lazy<ConcurrentBag<Timer>> timers = onceTimers.GetOrAdd(actorRef, (object actorRef) => new Lazy<ConcurrentBag<Timer>>());
        timers.Value.Add(ScheduleOnceTimer(actorRef, request, delay));
        return timers.Value;
    }

    /// <summary>
    /// Stops a periodic timer
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="NixieException"></exception>
    public void StopPeriodicTimer<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, string name)
        where TActor : IActor<TRequest> where TRequest : class
    {
        if (periodicTimers.TryGetValue(actorRef, out Lazy<ConcurrentDictionary<string, Lazy<Timer>>>? timers))
        {
            if (!timers.Value.TryGetValue(name, out Lazy<Timer>? timer))
                throw new NixieException("There is no timer with this name.");

            if (timer.IsValueCreated)
                timer.Value.Dispose();

            timers.Value.TryRemove(name, out _);
        }
    }

    /// <summary>
    /// Stops a periodic timer
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="NixieException"></exception>
    public void StopPeriodicTimer<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, string name)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        if (periodicTimers.TryGetValue(actorRef, out Lazy<ConcurrentDictionary<string, Lazy<Timer>>>? timers))
        {
            if (!timers.Value.TryGetValue(name, out Lazy<Timer>? timer))
                throw new NixieException("There is no timer with this name.");

            if (timer.IsValueCreated)
                timer.Value.Dispose();

            timers.Value.TryRemove(name, out _);
        }
    }

    /// <summary>
    /// Stops all timers running in an actor
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="actorRef"></param>
    public void StopAllTimers<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        StopAllTimersInternal(actorRef);
    }

    /// <summary>
    /// Stops all timers running in an actor
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="actorRef"></param>
    public void StopAllTimers<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef)
        where TActor : IActor<TRequest> where TRequest : class
    {
        StopAllTimersInternal(actorRef);
    }

    private Timer AddPeriodicTimerInternal<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest> where TRequest : class
    {
        return new((state) => SendScheduledMessage(actorRef, request), null, initialDelay, interval);
    }

    private Timer AddPeriodicTimerInternal<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        return new((state) => SendScheduledMessage(actorRef, request), null, initialDelay, interval);
    }

    private Timer ScheduleOnceTimer<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request, TimeSpan delay)
        where TActor : IActor<TRequest> where TRequest : class
    {
        return new((state) => SendScheduledMessage(actorRef, request), null, delay, TimeSpan.Zero);
    }

    private Timer ScheduleOnceTimer<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan delay)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        return new((state) => SendScheduledMessage(actorRef, request), null, delay, TimeSpan.Zero);
    }

    private void SendScheduledMessage<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request)
        where TActor : IActor<TRequest> where TRequest : class
    {
        actorRef.Send(request);
    }

    private void SendScheduledMessage<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        actorRef.Send(request);
    }

    private void StopAllTimersInternal(object actorRef)
    {
        if (periodicTimers.TryGetValue(actorRef, out Lazy<ConcurrentDictionary<string, Lazy<Timer>>>? actorPeriodicTimers))
        {
            periodicTimers.TryRemove(actorRef, out _);

            foreach (KeyValuePair<string, Lazy<Timer>> periodicTimer in actorPeriodicTimers.Value)
            {
                if (periodicTimer.Value.IsValueCreated)
                    periodicTimer.Value.Value?.Dispose();
            }
        }

        if (onceTimers.TryGetValue(actorRef, out Lazy<ConcurrentBag<Timer>>? actorOnceTimers))
        {
            onceTimers.TryRemove(actorRef, out _);

            if (!actorOnceTimers.IsValueCreated)
                return;

            foreach (Timer onceTimer in actorOnceTimers.Value)
                onceTimer?.Dispose();
        }
    }

    public void Dispose()
    {
        foreach (KeyValuePair<object, Lazy<ConcurrentDictionary<string, Lazy<Timer>>>> periodicTimer in periodicTimers)
        {
            foreach (KeyValuePair<string, Lazy<Timer>> timer in periodicTimer.Value.Value)
            {
                if (timer.Value.IsValueCreated)
                    timer.Value.Value.Dispose();
            }
        }

        foreach (KeyValuePair<object, Lazy<ConcurrentBag<Timer>>> onceTimer in onceTimers)
        {
            if (!onceTimer.Value.IsValueCreated)
                continue;

            foreach (Timer timer in onceTimer.Value.Value)
                timer?.Dispose();
        }
    }
}
