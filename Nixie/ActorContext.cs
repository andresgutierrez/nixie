
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// Represents an actor context. This class is passed to the actor when it is created.
/// It can be used to create other actors or get the sender and the actor system.
/// </summary>
public sealed class ActorContext<TActor, TRequest> : IActorContext<TActor, TRequest>
    where TActor : IActor<TRequest> where TRequest : class
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
    /// Returns a reference to the current actor
    /// </summary>
    public ActorRef<TActor, TRequest> Self { get; }

    /// <summary>
    /// Returns a reference to the sender of the message
    /// </summary>
    public IGenericActorRef? Sender { get; set; }

    /// <summary>
    /// Returns the actor runner
    /// </summary>
    public ActorRunner<TActor, TRequest>? Runner { get; set; }

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
    public ActorContext(ActorSystem actorSystem, ILogger? logger, ActorRef<TActor, TRequest> self)
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
