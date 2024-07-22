﻿
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// This context is passed to all created actors and allow them to interact with the actor system.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IActorContextStruct<TActor, TRequest, TResponse>
    where TActor : IActorStruct<TRequest, TResponse> where TRequest : struct where TResponse : struct
{
    /// <summary>
    /// Returns a reference to the actor system
    /// </summary>
    public ActorSystem ActorSystem { get; }

    /// <summary>
    /// Returns the actor system logger
    /// </summary>
    public ILogger? Logger { get; }

    /// <summary>
    /// Returns a reference to the current actor reference
    /// </summary>
    public ActorRefStruct<TActor, TRequest, TResponse> Self { get; }

    /// <summary>
    /// Returns a reference to the sender of the message
    /// </summary>
    public IGenericActorRef? Sender { get; set; }

    /// <summary>
    /// Returns a reference to the current reply object
    /// </summary>
    public ActorMessageReply<TRequest, TResponse>? Reply { get; set; }

    /// <summary>
    /// Indicates if the response of the current invocation must be by passed
    /// to allow other consumer to set the response
    /// </summary>
    public bool ByPassReply { get; set; }

    /// <summary>
    /// Returns the actor runner
    /// </summary>
    public ActorRunnerStruct<TActor, TRequest, TResponse>? Runner { get; set; }
}