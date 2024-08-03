
namespace Nixie.Tests.Actors;

public sealed class PostShutdownActor : IActor<string>
{
    private int receivedMessages;

    private bool shutdownFlag;

    public PostShutdownActor(IActorContext<PostShutdownActor, string> context)
    {
        context.OnPostShutdown += OnPostShutdown;
    }

    private void OnPostShutdown()
    {
        shutdownFlag = true;
    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    public bool GetShutdownFlag()
    {
        return shutdownFlag;
    }

    private void IncrMessage()
    {
        receivedMessages++;
    }

    public async Task Receive(string message)
    {
        await Task.Yield();

        IncrMessage();
    }
}