
namespace Nixie;

/// <summary>
/// Represents an actor context. This class is passed to the actor when it is created.
/// It can be used to create other actors or get the sender and the actor system.
/// </summary>
public class ActorContext : IActorContext
{
    private readonly ActorSystem actorSystem;

    /// <summary>
    /// Returns the actor system
    /// </summary>
    public ActorSystem ActorSystem => actorSystem;

    /// <summary>
    /// Creates a new actor context.
    /// </summary>
    /// <param name="actorSystem"></param>
    public ActorContext(ActorSystem actorSystem)
    {
        this.actorSystem = actorSystem;
    }
}
