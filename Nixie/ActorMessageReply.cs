
namespace Nixie;

/// <summary>
/// Represents a message sent to an actor that expects a reply.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed record ActorMessageReply<TRequest, TResponse>
{
    /// <summary>
    /// Returns the request of the message.
    /// </summary>
    public TRequest Request { get; }

    /// <summary>
    /// Returns the response of the message.
    /// </summary>
    public TResponse? Response { get; private set; }

    /// <summary>
    /// Returns true if the response has been set.
    /// </summary>
    public bool IsCompleted { get; private set; }    

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="request"></param>
    public ActorMessageReply(TRequest request)
	{
        Request = request;        
	}

    /// <summary>
    /// Marks the reply as completed.
    /// </summary>
    /// <param name="response"></param>
    public void SetCompleted(TResponse? response)
    {
        Response = response;
        IsCompleted = true;
    }
}

