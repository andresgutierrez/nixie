
namespace Nixie;

/// <summary>
/// Represents an actor reference that accepts struct messages.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public interface IActorRefStruct<TActor, TRequest> where TActor : IActorStruct<TRequest> where TRequest : struct
{
    /// <summary>
    /// The actor runner.
    /// </summary>
    public ActorRunnerStruct<TActor, TRequest> Runner { get; }

    /// <summary>
    /// Sends a message to the actor
    /// </summary>
    /// <param name="message"></param>
    public void Send(TRequest message);

    /// <summary>
    /// Sends a message to the actor
    /// </summary>
    /// <param name="message"></param>
    public void Send(TRequest message, IGenericActorRef sender);
}
