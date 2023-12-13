
namespace Nixie.Tests.Actors;

public enum OnceTimerMessageType
{
    Scheduled = 0
}

public sealed record OnceTimerMessage
{
    public OnceTimerMessage(OnceTimerMessageType type)
    {

    }
}

public sealed class OnceTimerActor : IActor<OnceTimerMessage>
{
    private int receivedMessages;

    public OnceTimerActor(IActorContext<OnceTimerActor, OnceTimerMessage> context)
    {
        context.ActorSystem.ScheduleOnce(context.Self, new OnceTimerMessage(OnceTimerMessageType.Scheduled), TimeSpan.FromSeconds(1));
    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    public void IncrMessage()
    {
        receivedMessages++;
    }

    public Task Receive(OnceTimerMessage message)
    {        
        IncrMessage();

        return Task.CompletedTask;
    }
}