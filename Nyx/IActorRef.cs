
namespace Nyx;

public interface IActorRef<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public void Send(TRequest message);
}
