
namespace Nyx;

public interface IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public Task<TResponse> Receive(TRequest message);
}
