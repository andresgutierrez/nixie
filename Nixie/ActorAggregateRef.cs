namespace Nixie;

/// <summary>
/// Represents an actor reference.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public sealed class ActorAggregateRef<TActor, TRequest> : IGenericActorRef, IActorAggregateRef<TActor, TRequest>
    where TActor : IActorAggregate<TRequest> where TRequest : class
{
    private readonly ActorRunnerAggregate<TActor, TRequest> runner;

    /// <summary>
    /// Returns the actor runner.
    /// </summary>
    public ActorRunnerAggregate<TActor, TRequest> Runner => runner;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="runner"></param>
    public ActorAggregateRef(ActorRunnerAggregate<TActor, TRequest> runner)
    {
        this.runner = runner;
    }

    /// <summary>
    /// Sends a message to the actor without expecting response
    /// </summary>
    /// <param name="message"></param>
    public void Send(TRequest message)
    {
        runner.SendAndTryDeliver(message, null);
    }

    /// <summary>
    /// Sends a message to the actor without expecting response
    /// and specifying the sender
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    public void Send(TRequest message, IGenericActorRef sender)
    {
        runner.SendAndTryDeliver(message, sender);
    }
}