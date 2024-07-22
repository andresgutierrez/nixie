
namespace Nixie.Tests.Actors;

public sealed class ReplyActorStruct : IActorStruct<int, int>
{
    private readonly Dictionary<int, int> receivedMessages = new();

    public ReplyActorStruct(IActorContextStruct<ReplyActorStruct, int, int> _)
    {

    }

    public int GetMessages(int id)
    {
        if (receivedMessages.TryGetValue(id, out int number))
            return number;

        return 0;
    }

    public void IncrMessage(int id)
    {
        if (!receivedMessages.TryGetValue(id, out int value))
            receivedMessages.Add(id, 1);
        else
            receivedMessages[id] = ++value;
    }

    public Task<int> Receive(int message)
    {
        IncrMessage(message);

        return Task.FromResult<int>(message);
    }
}