
namespace Nixie.Tests.Actors;

public sealed class RouteeSlowActor : IActor<RouterMessage>
{
    private int receivedMessages;

    private readonly IActorContext<RouteeSlowActor, RouterMessage> context;

    public RouteeSlowActor(IActorContext<RouteeSlowActor, RouterMessage> context)
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

    public async Task Receive(RouterMessage message)
    {        
        await Task.Delay(Random.Shared.Next(100, 1500));

        if (message.Type == RouterMessageType.Route)
            IncrMessage();
    }
}