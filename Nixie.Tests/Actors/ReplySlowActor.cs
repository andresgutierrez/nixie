
namespace Nixie.Tests.Actors;

public sealed class ReplySlowActor : IActor<string, string>
{
    private readonly Dictionary<string, int> receivedMessages = new();

    public ReplySlowActor(IActorContext<ReplySlowActor, string, string> _)
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

    public async Task<string?> Receive(string message)
    {
        IncrMessage(message);

        await Task.Delay(2000);

        return message;
    }
}