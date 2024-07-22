
using Microsoft.Extensions.Logging;

namespace Nixie;

/// <summary>
/// Represents an actor context. This class is passed to the actor when it is created.
/// It can be used to create other actors or get the sender and the actor system.
/// </summary>
public sealed class ActorContextStruct<TActor, TRequest, TResponse> : IActorContextStruct<TActor, TRequest, TResponse>
    where TActor : IActorStruct<TRequest, TResponse> where TRequest : struct where TResponse : struct
{
    private readonly ActorSystem actorSystem;

    private readonly ILogger? logger;

    private readonly ActorRefStruct<TActor, TRequest, TResponse> self;

    /// <summary>
    /// Returns the actor system
    /// </summary>
    public ActorSystem ActorSystem => actorSystem;

    /// <summary>
    /// Returns the actor system logger
    /// </summary>
    public ILogger? Logger => logger;

    /// <summary>
    /// Returns the actor system
    /// </summary>
    public ActorRefStruct<TActor, TRequest, TResponse> Self => self;

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

    /// <summary>
    /// Creates a new actor context.
    /// </summary>
    /// <param name="actorSystem"></param>
    /// <param name="logger"></param>
    /// <param name="self"></param>
    public ActorContextStruct(ActorSystem actorSystem, ILogger? logger, ActorRefStruct<TActor, TRequest, TResponse> self)
    {
        this.actorSystem = actorSystem;
        this.logger = logger;
        this.self = self;
    }
}
