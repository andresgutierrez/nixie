
namespace Nixie.Tests.Actors;

public sealed class PongActor : IActor<string, string>
{
    public PongActor(IActorContext<PongActor, string, string> _)
    {

    }

    public Task<string> Receive(string message)
    {        
        return Task.FromResult(message);
    }
}

public sealed class PingActor : IActor<string, string>
{
    private readonly IActorRef<PongActor, string, string> pongRef;

    private int receivedMessages;

    public PingActor(IActorContext<PingActor, string, string> context)
    {
        pongRef = context.ActorSystem.Spawn<PongActor, string, string>();
    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    public void IncrMessage()
    {
        receivedMessages++;
    }

    public async Task<string> Receive(string message)
    {
        IncrMessage();

        string? pongReply = await pongRef.Ask(message);

        return pongReply ?? "";
    }
}