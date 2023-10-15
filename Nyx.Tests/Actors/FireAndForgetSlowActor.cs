
namespace Nyx.Tests.Actors;

public sealed class FireAndForgetSlowActor : IActor<string>
{
    private readonly Dictionary<string, int> receivedMessages = new();

    public FireAndForgetSlowActor(IActorContext context)
    {

    }    

    public int GetMessages(string id)
    {
        if (receivedMessages.TryGetValue(id, out int number))
            return number;

        return 0;
    }

    public void IncrMessage(string id)
    {
        if (!receivedMessages.ContainsKey(id))
            receivedMessages.Add(id, 1);
        else
            receivedMessages[id]++;
    }

    public async Task Receive(string message)
    {
        await Task.Delay(1000);

        IncrMessage(message);
    }
}