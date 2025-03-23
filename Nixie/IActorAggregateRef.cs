namespace Nixie;

/// <summary>
/// Represents an actor reference.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public interface IActorAggregateRef<TActor, TRequest> where TActor : IActorAggregate<TRequest> where TRequest : class
{
    /// <summary>
    /// The actor runner.
    /// </summary>
    public ActorRunnerAggregate<TActor, TRequest> Runner { get; }

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