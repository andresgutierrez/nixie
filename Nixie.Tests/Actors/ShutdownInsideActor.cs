
namespace Nixie.Tests.Actors;

public class ShutdownInsideActor : IActor<string>
{
    private int receivedMessages;

    private readonly IActorContext<ShutdownInsideActor, string> context;

    public ShutdownInsideActor(IActorContext<ShutdownInsideActor, string> context)
    {
        this.context = context;
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
        await Task.Yield();

        if (message == "shutdown")
            context.ActorSystem.Shutdown(context.Self);
        else
            IncrMessage();
    }
}
