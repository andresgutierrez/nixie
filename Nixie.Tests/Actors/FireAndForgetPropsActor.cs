
namespace Nixie.Tests.Actors;

public sealed class FireAndForgetPropsSettings
{
    public int SomeSeting => 100;

    public string SomeOtherSetting => "hello"; 
}

public sealed class FireAndForgetPropsActor : IActor<string>
{
    private readonly string parameter;

    private readonly Dictionary<string, int> receivedMessages = new();

    public FireAndForgetPropsActor(IActorContext<FireAndForgetPropsActor, string> _, string parameter)
    {
        this.parameter = parameter;
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

        //Console.WriteLine("hello");

        IncrMessage(message);
    }
}
