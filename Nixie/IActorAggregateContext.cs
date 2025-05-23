
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// This context is passed to all created actors and allow them to interact with the actor system.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public interface IActorAggregateContext<TActor, TRequest> 
    where TActor : IActorAggregate<TRequest> where TRequest : class
{
    /// <summary>
    /// Event that is triggered when the actor is shutdown
    /// </summary>
    public event Action? OnPostShutdown;

    /// <summary>
    /// Returns the actor system
    /// </summary>
    public ActorSystem ActorSystem { get; }

    /// <summary>
    /// Returns the actor system logger
    /// </summary>
    public ILogger? Logger { get; }

    /// <summary>
    /// Returns a reference to the current actor
    /// </summary>
    public ActorRefAggregate<TActor, TRequest> Self { get; }

    /// <summary>
    /// Returns a reference to the sender of the message
    /// </summary>
    public IGenericActorRef? Sender { get; set; }

    /// <summary>
    /// Returns the actor runner
    /// </summary>
    public ActorRunnerAggregate<TActor, TRequest>? Runner { get; set; }
}