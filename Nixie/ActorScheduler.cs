
using System.Collections.Concurrent;

namespace Nixie;

/// <summary>
/// Schedules messages to be sent to actors at a specified interval.
/// </summary>
public class ActorScheduler
{
    private readonly ConcurrentDictionary<string, Timer> timers = new();    

    public void AddPeriodicTimer<TActor, TRequest>(string name, IActorRef<TActor, TRequest> actorRef, TRequest request, TimeSpan initialDelay, TimeSpan interval) where TActor : IActor<TRequest> where TRequest : class
    {
        Timer timer = new((x) => SendPeriodicTimer(actorRef, request), null, initialDelay, interval);
        timers.TryAdd(name, timer);
    }

    private void SendPeriodicTimer<TActor, TRequest>(IActorRef<TActor, TRequest> actorRef, TRequest request) where TActor : IActor<TRequest> where TRequest : class
    {
        actorRef.Send(request);
    }  
}
