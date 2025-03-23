namespace Nixie.Tests.Actors;

public sealed class FireAndForgetSlowAggregateActor : IActorAggregate<string>
{
    private readonly Dictionary<string, int> receivedMessages = new();

    public FireAndForgetSlowAggregateActor(IActorAggregateContext<FireAndForgetSlowAggregateActor, string> context)
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

    public async Task Receive(List<string> messages)
    {
        await Task.Delay(100);

        foreach (string message in messages)
            IncrMessage(message);
    }
}