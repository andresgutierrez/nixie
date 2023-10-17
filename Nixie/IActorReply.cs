
namespace Nixie;

/// <summary>
/// This interface must be implemented by all actors that return a response.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IActor<TRequest, TResponse> where TRequest : class where TResponse : class?
{
    /// <summary>
    /// Passes a message to the actor and returns a response.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task<TResponse?> Receive(TRequest message);
}
