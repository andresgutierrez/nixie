
namespace Nixie;

/// <summary>
/// Represents a message sent to an actor that expects a reply.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed record ActorMessageReply<TRequest, TResponse>
{
    private int completed = 1;

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
    /// Returns true if the response has been set.
    /// </summary>
    public bool IsCompleted => completed == 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="request"></param>
    public ActorMessageReply(TRequest request, IGenericActorRef? sender)
    {
        Request = request;
        Sender = sender;
    }

    /// <summary>
    /// Marks the reply as completed.
    /// </summary>
    /// <param name="response"></param>
    public void SetCompleted(TResponse? response)
    {
        Interlocked.Exchange(ref completed, 0);
        Response = response;
    }
}

