
namespace Nixie;

public interface IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    public Task<TResponse> Receive(TRequest message);
}

public interface IActor<TRequest> where TRequest : class
{
    public Task Receive(TRequest message);
}
