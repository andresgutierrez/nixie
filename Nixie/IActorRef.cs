
namespace Nixie;

public interface IActorRef<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    public ActorRunner<TActor, TRequest> Runner { get; }

    public void Send(TRequest message);
}
