
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// Represents an actor context. This class is passed to the actor when it is created.
/// It can be used to create other actors or get the sender and the actor system.
/// </summary>
public sealed class ActorContext<TActor, TRequest, TResponse> : IActorContext<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    private readonly ActorSystem actorSystem;

    private readonly ILogger? logger;

    private readonly ActorRef<TActor, TRequest, TResponse> self;

    /// <summary>
    /// Returns the actor system
    /// </summary>
    public ActorSystem ActorSystem => actorSystem;

    /// <summary>
    /// Returns the actor system logger
    /// </summary>
    public ILogger? Logger => logger;

    /// <summary>
    /// Returns the actor system
    /// </summary>
    public ActorRef<TActor, TRequest, TResponse> Self => self;

    /// <summary>
    /// Returns a reference to the sender of the message
    /// </summary>
    public IGenericActorRef? Sender { get; set; }

    /// <summary>
    /// Creates a new actor context.
    /// </summary>
    /// <param name="actorSystem"></param>
    /// <param name="logger"></param>
    /// <param name="self"></param>
    public ActorContext(ActorSystem actorSystem, ILogger? logger, ActorRef<TActor, TRequest, TResponse> self)
    {
        this.actorSystem = actorSystem;
        this.logger = logger;
        this.self = self;
    }
}
