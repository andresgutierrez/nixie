
namespace Nixie;

/// <summary>
/// This interface must be implemented by all actors that do not return a response.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IActor<in TRequest> where TRequest : class
{
    public Task Receive(TRequest message);
}
