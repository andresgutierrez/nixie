
namespace Nixie.Tests.Actors;

public class MessageCountRequest
{
	
}

public class MessageCountResponse
{
    public int? Counter { get; }

    public MessageCountResponse(int? counter)
    {
        Counter = counter;
    }
}

public sealed class MessageCountActor : IActor<MessageCountRequest, MessageCountResponse>
{
    private readonly IActorContext<MessageCountActor, MessageCountRequest, MessageCountResponse> context;

    public MessageCountActor(IActorContext<MessageCountActor, MessageCountRequest, MessageCountResponse> context)
    {
        this.context = context;
    }

    public Task<MessageCountResponse?> Receive(MessageCountRequest message)
    {
        return Task.FromResult<MessageCountResponse?>(new(context.Runner?.MessageCount));
    }
}

