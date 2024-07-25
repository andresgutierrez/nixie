
namespace Nixie.Tests.Actors;

public sealed class PeriodicTimerActorStruct : IActorStruct<int>
{
    private readonly Dictionary<int, int> receivedMessages = new();

    public PeriodicTimerActorStruct(IActorContextStruct<PeriodicTimerActorStruct, int> context)
    {
        context.ActorSystem.StartPeriodicTimerStruct(context.Self, "periodic-timer", 100, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    public int GetMessages(int id)
    {
        return receivedMessages.GetValueOrDefault(id, 0);
    }

    private void IncrMessage(int id)
    {
        if (!receivedMessages.TryGetValue(id, out int value))
            receivedMessages.Add(id, 1);
        else
            receivedMessages[id] = ++value;
    }

    public async Task Receive(int message)
    {
        await Task.CompletedTask;

        IncrMessage(message);
    }
}