
namespace Nixie.Tests.Actors;

public sealed class ShutdownSlowActor : IActor<string>
{
    private int receivedMessages;

    public ShutdownSlowActor(IActorContext<ShutdownSlowActor, string> _)
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

    public async Task Receive(string message)
    {
        await Task.Delay(1000);

        IncrMessage();
    }
}