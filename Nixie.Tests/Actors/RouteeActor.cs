
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
        ulong hash = 14695981039346656037;

        ReadOnlySpan<char> spanBytes = Data.AsSpan();

        for (int i = 0; i < spanBytes.Length; i++)
        {
            unchecked
            {
                hash ^= spanBytes[i];
                hash *= 0x100000001b3;
            }
        }

        return Math.Abs((int)hash);
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
