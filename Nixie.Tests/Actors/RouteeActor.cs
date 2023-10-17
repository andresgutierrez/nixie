
using Nixie.Routers;

namespace Nixie.Tests.Actors;

public enum RouterMessageType
{
    Route = 0
}

public sealed class RouterMessage : IConsistentHashable
{
    public RouterMessageType Type { get; }

    public string Data { get; }

    public RouterMessage(RouterMessageType type, string data)
    {
        Type = type;
        Data = data;
    }

    public int GetHash()
    {
        return Data switch
        {
            "aaa" => 0,
            "bbb" => 1,
            "ccc" => 2,
            "ddd" => 3,
            "eee" => 4,
            _ => 0
        };
    }
}

public sealed class RouteeActor : IActor<RouterMessage>
{
    private int receivedMessages;

    private readonly IActorContext<RouteeActor, RouterMessage> context;

    public RouteeActor(IActorContext<RouteeActor, RouterMessage> context)
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

    public async Task Receive(RouterMessage message)
    {
        await Task.Yield();

        if (message.Type == RouterMessageType.Route)
            IncrMessage();
    }
}
