
namespace Nixie;

/// <summary>
/// This interface must be implemented by all actors that return a response.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IActorStruct<in TRequest, TResponse> where TRequest : struct where TResponse : struct
{
    /// <summary>
    /// Passes a message to the actor and returns a response.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task<TResponse> Receive(TRequest message);
}