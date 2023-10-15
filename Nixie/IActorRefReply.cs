
namespace Nixie;

public interface IActorRef<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public ActorRunner<TActor, TRequest, TResponse> Runner { get; }

    public void Send(TRequest message);

    public Task<TResponse?> Ask(TRequest message);
}
