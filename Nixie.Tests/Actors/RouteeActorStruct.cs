
using Nixie.Routers;

namespace Nixie.Tests.Actors;

public readonly struct RouterMessageStruct : IConsistentHashable
{
    public RouterMessageType Type { get; }

    public string Data { get; }

    public RouterMessageStruct(RouterMessageType type, string data)
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

public sealed class RouteeActorStruct : IActorStruct<RouterMessageStruct>
{
    private int receivedMessages;

    private readonly IActorContextStruct<RouteeActorStruct, RouterMessageStruct> context;

    public RouteeActorStruct(IActorContextStruct<RouteeActorStruct, RouterMessageStruct> context)
    {
        this.context = context;
    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    private void IncrMessage()
    {
        receivedMessages++;
    }

    public async Task Receive(RouterMessageStruct message)
    {
        await Task.Yield();

        if (message.Type == RouterMessageType.Route)
            IncrMessage();
    }
}
