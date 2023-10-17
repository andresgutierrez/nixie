
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
    /// Sends a message to the actor without expecting response
    /// </summary>
    /// <param name="message"></param>
    /// <param name="message"></param>
    public void Send(TRequest message, IGenericActorRef sender);

    /// <summary>
    /// Sends a message to the actor and expects a response
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task<TResponse?> Ask(TRequest message);

    /// <summary>
    /// Sends a message to the actor and expects a response
    /// An exception will be thrown if the timeout limit is reached
    /// </summary>
    /// <param name="message"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public Task<TResponse?> Ask(TRequest message, TimeSpan timeout);

    /// <summary>
    /// Sends a message to the actor and expects a response
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public Task<TResponse?> Ask(TRequest message, IGenericActorRef sender);

    /// <summary>
    /// Sends a message to the actor and expects a response
    /// An exception will be thrown if the timeout limit is reached
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public Task<TResponse?> Ask(TRequest message, IGenericActorRef sender, TimeSpan timeout);
}
