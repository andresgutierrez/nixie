
namespace Nixie;

public interface IActorContext<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    public ActorSystem ActorSystem { get; }

    public IActorRef<TActor, TRequest> Self { get; }
}
