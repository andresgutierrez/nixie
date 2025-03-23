
namespace Nixie.Tests.Actors;

public sealed class FireAndForgetAggregateActor : IActorAggregate<string>
{
    private readonly Dictionary<string, int> receivedMessages = new();

    public FireAndForgetAggregateActor(IActorAggregateContext<FireAndForgetAggregateActor, string> _)
    {

    }

    public int GetMessages(string id)
    {
        return receivedMessages.GetValueOrDefault(id, 0);
    }

    private void IncrMessage(string id)
    {
        if (!receivedMessages.TryGetValue(id, out int value))
            receivedMessages.Add(id, 1);
        else
            receivedMessages[id] = ++value;
    }

    public Task Receive(List<string> messages)
    {
        foreach (string message in messages)
            IncrMessage(message);
        
        return Task.CompletedTask;
    }
}