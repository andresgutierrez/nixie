
namespace Nixie;

/// <summary>
/// Represents an actor reference.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public interface IActorRef<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    /// <summary>
    /// The actor runner.
    /// </summary>
    public ActorRunner<TActor, TRequest> Runner { get; }

    /// <summary>
    /// Sends a message to the actor
    /// </summary>
    /// <param name="message"></param>
    public void Send(TRequest message);
}
