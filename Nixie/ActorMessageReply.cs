using DotNext.Threading.Tasks;

namespace Nixie;

/// <summary>
/// Represents a message sent to an actor that expects a reply.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public readonly struct ActorMessageReply<TRequest, TResponse>
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
    /// Returns the response of the message.
    /// </summary>
    public TResponse? Response { get; private set; }

    /// <summary>
    /// Returns the task completion source of the reply
    /// </summary>
    public ValueTaskCompletionSource<TResponse?> Promise { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="request"></param>
    /// <param name="sender"></param>
    /// <param name="promise"></param>
    public ActorMessageReply(TRequest request, IGenericActorRef? sender, ValueTaskCompletionSource<TResponse?> promise)
    {
        Request = request;
        Sender = sender;
        Promise = promise;
    }
}
