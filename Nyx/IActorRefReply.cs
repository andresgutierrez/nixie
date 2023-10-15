
namespace Nyx;

public interface IActorRef<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public ActorContext<TActor, TRequest, TResponse> Context { get; }

    public void Send(TRequest message);

    public Task<TResponse?> Ask(TRequest message);
}
