
namespace Nixie;

public sealed record ActorMessageReply<TRequest, TResponse>
{
    public TRequest Request { get; }

    public TResponse? Response { get; private set; }

    public bool IsCompleted { get; private set; }    

    public ActorMessageReply(TRequest request)
	{
        Request = request;        
	}

    public void SetCompleted(TResponse? response)
    {
        Response = response;
        IsCompleted = true;
    }
}

