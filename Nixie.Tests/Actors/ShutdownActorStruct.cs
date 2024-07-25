
namespace Nixie.Tests.Actors;

public sealed class ShutdownActorStruct : IActorStruct<int>
{
    private int receivedMessages;

    public ShutdownActorStruct(IActorContextStruct<ShutdownActorStruct, int> _)
    {

    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    private void IncrMessage()
    {
        receivedMessages++;
    }

    public async Task Receive(int message)
    {
        await Task.Yield();

        IncrMessage();
    }
}
