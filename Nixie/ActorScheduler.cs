﻿
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// Schedules messages to be sent to actors at a specified interval.
/// </summary>
public class ActorScheduler : IDisposable
{
    private static int sequence;

    private readonly ActorSystem actorSystem;

    private readonly ILogger? logger;

    private readonly ConcurrentDictionary<object, Lazy<ConcurrentDictionary<long, Lazy<Timer>>>> onceTimers = new();

    private readonly ConcurrentDictionary<object, Lazy<ConcurrentDictionary<string, Lazy<Timer>>>> periodicTimers = new();

    public ActorScheduler(ActorSystem actorSystem, ILogger? logger)
    {
        this.actorSystem = actorSystem;
        this.logger = logger;
    }

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

        Lazy<ConcurrentDictionary<string, Lazy<Timer>>> timers = periodicTimers.GetOrAdd(actorRef, (_) => new());
        Lazy<Timer> timer = timers.Value.GetOrAdd(name, (string _) => new(() => AddPeriodicTimerInternal(actorRef, request, initialDelay, interval)));
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
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class?
    {
        if (periodicTimers.ContainsKey(name))
            throw new NixieException("There is already an active timer with this name.");

        Lazy<ConcurrentDictionary<string, Lazy<Timer>>> timers = periodicTimers.GetOrAdd(actorRef, (_) => new());
        Lazy<Timer> timer = timers.Value.GetOrAdd(name, (string _) => new(() => AddPeriodicTimerInternal(actorRef, request, initialDelay, interval)));
        return timer.Value;
    }
    
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
    public Timer StartPeriodicTimerStruct<TActor, TRequest>(IActorRefStruct<TActor, TRequest> actorRef, string name, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActorStruct<TRequest> where TRequest : struct
    {
        if (periodicTimers.ContainsKey(name))
            throw new NixieException("There is already an active timer with this name.");

        Lazy<ConcurrentDictionary<string, Lazy<Timer>>> timers = periodicTimers.GetOrAdd(actorRef, (_) => new());
        Lazy<Timer> timer = timers.Value.GetOrAdd(name, (string _) => new(() => AddPeriodicTimerInternalStruct(actorRef, request, initialDelay, interval)));
        return timer.Value;
    }
    
    /// <summary>
    /// Schedules a message to be sent to an actor once after a specified delay at a specified interval.
    /// </summary>
    /// <param name="actorRef"></param>
    /// <param name="name"></param>
    /// <param name="request"></param>
    /// <param name="initialDelay"></param>
    /// <param name="interval"></param>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    /// <exception cref="NixieException"></exception>
    public Timer StartPeriodicTimerStruct<TActor, TRequest, TResponse>(IActorRefStruct<TActor, TRequest, TResponse> actorRef, string name, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActorStruct<TRequest, TResponse> where TRequest : struct where TResponse : struct
    {
        if (periodicTimers.ContainsKey(name))
            throw new NixieException("There is already an active timer with this name.");

        Lazy<ConcurrentDictionary<string, Lazy<Timer>>> timers = periodicTimers.GetOrAdd(actorRef, (_) => new());
        Lazy<Timer> timer = timers.Value.GetOrAdd(name, (string _) => new(() => AddPeriodicTimerInternalStruct(actorRef, request, initialDelay, interval)));
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
    public Timer ScheduleOnce<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan delay)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class?
    {
        long seq = Interlocked.Increment(ref sequence);
        Lazy<ConcurrentDictionary<long, Lazy<Timer>>> timers = onceTimers.GetOrAdd(actorRef, (object _) => new());
        Lazy<Timer> timer = timers.Value.GetOrAdd(seq, (long key) => new(() => ScheduleOnceTimer(actorRef, request, delay, seq)));
        return timer.Value;
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
    public Timer ScheduleOnce<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request, TimeSpan delay)
        where TActor : IActor<TRequest> where TRequest : class
    {
        long seq = Interlocked.Increment(ref sequence);
        Lazy<ConcurrentDictionary<long, Lazy<Timer>>> timers = onceTimers.GetOrAdd(actorRef, (object _) => new());
        Lazy<Timer> timer = timers.Value.GetOrAdd(seq, (long key) => new(() => ScheduleOnceTimer(actorRef, request, delay, seq)));
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
    public Timer ScheduleOnceStruct<TActor, TRequest, TResponse>(IActorRefStruct<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan delay)
        where TActor : IActorStruct<TRequest, TResponse> where TRequest : struct where TResponse : struct
    {
        long seq = Interlocked.Increment(ref sequence);
        Lazy<ConcurrentDictionary<long, Lazy<Timer>>> timers = onceTimers.GetOrAdd(actorRef, (object _) => new());
        Lazy<Timer> timer = timers.Value.GetOrAdd(seq, (long key) => new(() => ScheduleOnceTimerStruct(actorRef, request, delay, seq)));
        return timer.Value;
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
    public Timer ScheduleOnceStruct<TActor, TRequest>(IActorRefStruct<TActor, TRequest> actorRef, TRequest request, TimeSpan delay)
        where TActor : IActorStruct<TRequest> where TRequest : struct
    {
        long seq = Interlocked.Increment(ref sequence);
        Lazy<ConcurrentDictionary<long, Lazy<Timer>>> timers = onceTimers.GetOrAdd(actorRef, (object _) => new());
        Lazy<Timer> timer = timers.Value.GetOrAdd(seq, (long key) => new(() => ScheduleOnceTimerStruct(actorRef, request, delay, seq)));
        return timer.Value;
    }
    
    /// <summary>
    /// Schedule an actor to be terminated after the specified delay.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="actorRef"></param>
    /// <param name="request"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public Timer ScheduleShutdown<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TimeSpan delay)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class?
    {
        return new((state) => actorSystem.Shutdown(actorRef), null, delay, TimeSpan.Zero);
    }
    
    /// <summary>
    /// Schedule an actor to be terminated after the specified delay.
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="actorRef"></param>
    /// <param name="request"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public Timer ScheduleShutdown<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TimeSpan delay)
        where TActor : IActor<TRequest> where TRequest : class
    {
        return new((state) => actorSystem.Shutdown(actorRef), null, delay, TimeSpan.Zero);
    }

    /// <summary>
    /// Stops a periodic timer
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="NixieException"></exception>
    public void StopPeriodicTimer<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, string name) where TActor : IActor<TRequest> where TRequest : class
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
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class?
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
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class?
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

    /// <summary>
    /// Stops all timers running in an actor
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="actorRef"></param>
    public void StopAllTimers<TActor, TRequest>(IActorRefStruct<TActor, TRequest> actorRef)
        where TActor : IActorStruct<TRequest> where TRequest : struct
    {
        StopAllTimersInternal(actorRef);
    }

    /// <summary>
    /// Stops all timers running in an actor
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="actorRef"></param>
    public void StopAllTimers<TActor, TRequest, TResponse>(IActorRefStruct<TActor, TRequest, TResponse> actorRef)
        where TActor : IActorStruct<TRequest, TResponse> where TRequest : struct where TResponse : struct
    {
        StopAllTimersInternal(actorRef);
    }

    private Timer AddPeriodicTimerInternal<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest> where TRequest : class
    {
        return new((state) => SendScheduledMessage(actorRef, request, -1), null, initialDelay, interval);
    }

    private Timer AddPeriodicTimerInternal<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class?
    {
        return new((state) => SendScheduledMessage(actorRef, request, -1), null, initialDelay, interval);
    }
    
    private Timer AddPeriodicTimerInternalStruct<TActor, TRequest>(IActorRefStruct<TActor, TRequest> actorRef, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActorStruct<TRequest> where TRequest : struct
    {
        return new((state) => SendScheduledMessage(actorRef, request, -1), null, initialDelay, interval);
    }
    
    private Timer AddPeriodicTimerInternalStruct<TActor, TRequest, TResponse>(IActorRefStruct<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan initialDelay, TimeSpan interval)
        where TActor : IActorStruct<TRequest, TResponse> where TRequest : struct where TResponse : struct
    {
        return new((state) => SendScheduledMessage(actorRef, request, -1), null, initialDelay, interval);
    }

    private Timer ScheduleOnceTimer<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request, TimeSpan delay, long random)
        where TActor : IActor<TRequest> where TRequest : class
    {
        return new((state) => SendScheduledMessage(actorRef, request, random), null, delay, TimeSpan.Zero);
    }

    private Timer ScheduleOnceTimer<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan delay, long random)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class?
    {
        return new((state) => SendScheduledMessage(actorRef, request, random), null, delay, TimeSpan.Zero);
    }

    private Timer ScheduleOnceTimerStruct<TActor, TRequest>(IActorRefStruct<TActor, TRequest> actorRef, TRequest request, TimeSpan delay, long random)
       where TActor : IActorStruct<TRequest> where TRequest : struct
    {
        return new((state) => SendScheduledMessage(actorRef, request, random), null, delay, TimeSpan.Zero);
    }

    private Timer ScheduleOnceTimerStruct<TActor, TRequest, TResponse>(IActorRefStruct<TActor, TRequest, TResponse> actorRef, TRequest request, TimeSpan delay, long random)
        where TActor : IActorStruct<TRequest, TResponse> where TRequest : struct where TResponse : struct
    {
        return new((state) => SendScheduledMessage(actorRef, request, random), null, delay, TimeSpan.Zero);
    }

    private void SendScheduledMessage<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request, long random)
        where TActor : IActor<TRequest> where TRequest : class
    {
        try
        {
            actorRef.Send(request);

            if (random > -1 && onceTimers.TryGetValue(actorRef, out Lazy<ConcurrentDictionary<long, Lazy<Timer>>>? timers))
            {
                if (!timers.Value.TryGetValue(random, out Lazy<Timer>? timer))
                    return;

                if (timer.IsValueCreated)
                    timer.Value.Dispose();

                timers.Value.TryRemove(random, out _);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError("{Ex}", ex.Message);
        }
    }

    private void SendScheduledMessage<TActor, TRequest, TResponse>(IActorRef<TActor, TRequest, TResponse> actorRef, TRequest request, long random)
        where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class?
    {
        try
        {
            actorRef.Send(request);

            if (random > -1 && onceTimers.TryGetValue(actorRef, out Lazy<ConcurrentDictionary<long, Lazy<Timer>>>? timers))
            {
                if (!timers.Value.TryGetValue(random, out Lazy<Timer>? timer))
                    return;

                if (timer.IsValueCreated)
                    timer.Value.Dispose();

                timers.Value.TryRemove(random, out _);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError("{Ex}", ex.Message);
        }
    }
    
    private void SendScheduledMessage<TActor, TRequest>(IActorRefStruct<TActor, TRequest> actorRef, TRequest request, long random)
        where TActor : IActorStruct<TRequest> where TRequest : struct
    {
        try
        {
            actorRef.Send(request);

            if (random > -1 && onceTimers.TryGetValue(actorRef, out Lazy<ConcurrentDictionary<long, Lazy<Timer>>>? timers))
            {
                if (!timers.Value.TryGetValue(random, out Lazy<Timer>? timer))
                    return;

                if (timer.IsValueCreated)
                    timer.Value.Dispose();

                timers.Value.TryRemove(random, out _);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError("{Ex}", ex.Message);
        }
    }
    
    private void SendScheduledMessage<TActor, TRequest, TResponse>(IActorRefStruct<TActor, TRequest, TResponse> actorRef, TRequest request, long random)
        where TActor : IActorStruct<TRequest, TResponse> where TRequest : struct where TResponse : struct
    {
        try
        {
            actorRef.Send(request);

            if (random > -1 && onceTimers.TryGetValue(actorRef, out Lazy<ConcurrentDictionary<long, Lazy<Timer>>>? timers))
            {
                if (!timers.Value.TryGetValue(random, out Lazy<Timer>? timer))
                    return;

                if (timer.IsValueCreated)
                    timer.Value.Dispose();

                timers.Value.TryRemove(random, out _);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError("{Ex}", ex.Message);
        }
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

        if (onceTimers.TryGetValue(actorRef, out Lazy<ConcurrentDictionary<long, Lazy<Timer>>>? actorOnceTimers))
        {
            onceTimers.TryRemove(actorRef, out _);

            if (!actorOnceTimers.IsValueCreated)
                return;

            foreach (KeyValuePair<long, Lazy<Timer>> onceTimer in actorOnceTimers.Value)
            {
                Lazy<Timer>? lazyTimer = onceTimer.Value;
                if (lazyTimer is null)
                    continue;

                if (!lazyTimer.IsValueCreated)
                    continue;

                lazyTimer.Value.Dispose();
            }
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

        foreach (KeyValuePair<object, Lazy<ConcurrentDictionary<long, Lazy<Timer>>>> actorOnceTimer in onceTimers)
        {
            if (!actorOnceTimer.Value.IsValueCreated)
                continue;

            foreach (KeyValuePair<long, Lazy<Timer>> onceTimer in actorOnceTimer.Value.Value)
            {
                Lazy<Timer>? lazyTimer = onceTimer.Value;
                if (lazyTimer is null)
                    continue;

                if (!lazyTimer.IsValueCreated)
                    continue;

                lazyTimer.Value.Dispose();
            }
        }
    }
}
