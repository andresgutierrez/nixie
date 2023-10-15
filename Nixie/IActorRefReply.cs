
namespace Nixie;

/// <summary>
/// Represents an actor reference.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IActorRef<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    /// <summary>
    /// The actor runner.
    /// </summary>
    public ActorRunner<TActor, TRequest, TResponse> Runner { get; }

    /// <summary>
    /// Sends a message to the actor without expecting response
    /// </summary>
    /// <param name="message"></param>
    public void Send(TRequest message);

    /// <summary>
    /// Sends a message to the actor and expects a response
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task<TResponse?> Ask(TRequest message);
}
