﻿
namespace Nixie;

/// <summary>
/// Represents an actor reference.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public sealed class ActorRef<TActor, TRequest> : IGenericActorRef, IActorRef<TActor, TRequest>
    where TActor : IActor<TRequest> where TRequest : class
{
    private readonly ActorRunner<TActor, TRequest> runner;

    /// <summary>
    /// Returns the actor runner.
    /// </summary>
    public ActorRunner<TActor, TRequest> Runner => runner;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="runner"></param>
    public ActorRef(ActorRunner<TActor, TRequest> runner)
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
