
namespace Nixie;

/// <summary>
/// Represents a message sent to an actor that doesn't expect a reply.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public readonly struct ActorMessage<TRequest>
{
    /// <summary>
    /// Returns the request of the message.
    /// </summary>
    public TRequest Request { get; }

    /// <summary>
    /// The sender of the message
    /// </summary>
    public IGenericActorRef? Sender { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="request"></param>
    public ActorMessage(TRequest request, IGenericActorRef? sender)
    {
        Request = request;
        Sender = sender;
    }
}

