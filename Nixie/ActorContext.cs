
namespace Nixie;

/// <summary>
/// Represents an actor context. This class is passed to the actor when it is created.
/// It can be used to create other actors or get the sender and the actor system.
/// </summary>
public sealed class ActorContext<TActor, TRequest> : IActorContext<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    private readonly ActorSystem actorSystem;

    private readonly ActorRef<TActor, TRequest> self;

    /// <summary>
    /// Returns the actor system
    /// </summary>
    public ActorSystem ActorSystem => actorSystem;

    /// <summary>
    /// Returns a reference to the current actor
    /// </summary>
    public ActorRef<TActor, TRequest> Self => self;

    /// <summary>
    /// Returns a reference to the sender of the message
    /// </summary>
    public IGenericActorRef? Sender { get; set; }

    /// <summary>
    /// Creates a new actor context.
    /// </summary>
    /// <param name="actorSystem"></param>
    public ActorContext(ActorSystem actorSystem, ActorRef<TActor, TRequest> self)
    {
        this.actorSystem = actorSystem;
        this.self = self;
    }
}
