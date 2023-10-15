
namespace Nixie;

public interface IActorContext<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public ActorSystem ActorSystem { get; }

    public IActorRef<TActor, TRequest, TResponse> Self { get; }
}
