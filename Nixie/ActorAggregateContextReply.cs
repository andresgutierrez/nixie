
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// Represents an actor context. This class is passed to the actor when it is created.
/// It can be used to create other actors or get the sender and the actor system.
/// </summary>
public sealed class ActorAggregateContext<TActor, TRequest, TResponse> : IActorAggregateContext<TActor, TRequest, TResponse>
    where TActor : IActorAggregate<TRequest, TResponse> where TRequest : class where TResponse : class?
{
    /// <summary>
    /// Returns the actor system
    /// </summary>
    public ActorSystem ActorSystem { get; }

    /// <summary>
    /// Returns the actor system logger
    /// </summary>
    public ILogger? Logger { get; }

    /// <summary>
    /// Returns the actor reference
    /// </summary>
    public ActorRefAggregate<TActor, TRequest, TResponse> Self { get; }

    /// <summary>
    /// Returns a reference to the sender of the message
    /// </summary>
    public IGenericActorRef? Sender { get; set; }

    /// <summary>
    /// Returns a reference to the current reply object
    /// </summary>
    public ActorMessageReply<TRequest, TResponse>? Reply { get; set; }

    /// <summary>
    /// Indicates if the response of the current invocation must be by passed
    /// to allow other consumer to set the response
    /// </summary>
    public bool ByPassReply { get; set; }

    /// <summary>
    /// Returns the actor runner
    /// </summary>
    public ActorRunnerAggregate<TActor, TRequest, TResponse>? Runner { get; set; }

    /// <summary>
    /// Event that is triggered when the actor is shutdown
    /// </summary>
    public event Action? OnPostShutdown;

    /// <summary>
    /// Creates a new actor context.
    /// </summary>
    /// <param name="actorSystem"></param>
    /// <param name="logger"></param>
    /// <param name="self"></param>
    public ActorAggregateContext(ActorSystem actorSystem, ILogger? logger, ActorRefAggregate<TActor, TRequest, TResponse> self)
    {
        ActorSystem = actorSystem;
        Logger = logger;
        Self = self;
    }

    /// <summary>
    /// Run the post shutdown routine for the actor
    /// </summary>
    public void PostShutdown()
    {
        OnPostShutdown?.Invoke();
    }
}