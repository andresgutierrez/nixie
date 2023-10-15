
namespace Nixie;

public class ActorContext : IActorContext
{
    private readonly ActorSystem actorSystem;

    public ActorSystem ActorSystem => actorSystem;

    public ActorContext(ActorSystem actorSystem)
    {
        this.actorSystem = actorSystem;
    }
}
