
namespace Nyx;

public interface IActorRef<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    public ActorContext<TActor, TRequest> Context { get; }

    public void Send(TRequest message);
}
