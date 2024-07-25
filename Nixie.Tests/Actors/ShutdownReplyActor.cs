namespace Nixie.Tests.Actors;

public sealed class ShutdownReplyActor : IActor<string, string>
{
    private int receivedMessages;

    public ShutdownReplyActor(IActorContext<ShutdownReplyActor, string, string> _)
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

    public async Task<string?> Receive(string message)
    {
        await Task.Yield();

        IncrMessage();

        return message;
    }
}