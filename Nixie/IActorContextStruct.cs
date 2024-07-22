﻿
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// This context is passed to all created actors and allow them to interact with the actor system.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public interface IActorContextStruct<TActor, TRequest> where TActor : IActorStruct<TRequest> where TRequest : struct
{
    /// <summary>
    /// Returns the actor system
    /// </summary>
    public ActorSystem ActorSystem { get; }

    /// <summary>
    /// Returns the actor system logger
    /// </summary>
    public ILogger? Logger { get; }

    /// <summary>
    /// Returns a reference to the current actor
    /// </summary>
    public ActorRefStruct<TActor, TRequest> Self { get; }

    /// <summary>
    /// Returns a reference to the sender of the message
    /// </summary>
    public IGenericActorRef? Sender { get; set; }

    /// <summary>
    /// Returns the actor runner
    /// </summary>
    public ActorRunnerStruct<TActor, TRequest>? Runner { get; set; }
}
