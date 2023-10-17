
namespace Nixie.Tests.Actors;

public sealed class RouterResponse
{
    public string Data { get; }

    public RouterResponse(string data)
    {
        Data = data;
    }
}

public sealed class RouteeReplyActor : IActor<RouterMessage, RouterResponse>
{
    private int receivedMessages;

    private readonly IActorContext<RouteeReplyActor, RouterMessage, RouterResponse> context;

    public RouteeReplyActor(IActorContext<RouteeReplyActor, RouterMessage, RouterResponse> context)
    {
        this.context = context;
    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    public void IncrMessage()
    {
        receivedMessages++;
    }

    public Task<RouterResponse?> Receive(RouterMessage message)
    {        
        if (message.Type == RouterMessageType.Route)
            IncrMessage();

        return Task.FromResult<RouterResponse?>(new RouterResponse(message.Data));
    }
}