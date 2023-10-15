
namespace Nixie.Tests.Actors;

public enum FaultyMessageType
{
    Ok = 0,
    Faulty = 1
}

public sealed class FaultyMessage
{
    public FaultyMessageType Type { get; }

    public FaultyMessage(FaultyMessageType type)
    {
        Type = type;
    }
}

public sealed class FaultyActor : IActor<FaultyMessage>
{
    private int receivedMessages;

    public FaultyActor(IActorContext<FaultyActor, FaultyMessage> context)
    {

    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    public void IncrMessage()
    {
        receivedMessages++;
    }

    public async Task Receive(FaultyMessage message)
    {
        await Task.Yield();

        if (message.Type == FaultyMessageType.Faulty)
            throw new Exception("Faulty message");

        IncrMessage();
    }
}
