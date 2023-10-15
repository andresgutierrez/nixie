
namespace Nixie.Tests.Actors;

public sealed class FireAndForgetPropsSettings
{    
    public string Id => "hello"; 
}

public sealed class FireAndForgetPropsActor : IActor<string>
{
    private readonly FireAndForgetPropsSettings settings;

    private readonly Dictionary<string, int> receivedMessages = new();

    public FireAndForgetPropsActor(IActorContext<FireAndForgetPropsActor, string> _, FireAndForgetPropsSettings settings)
    {
        this.settings = settings;
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
        await Task.Yield();

        IncrMessage(settings.Id);
    }
}
