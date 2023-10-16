
using Nixie.Actors;

namespace Nixie.Tests.Actors;

internal sealed class NobodySenderActor : IActor<SenderRequest>
{
    private readonly IActorContext<NobodySenderActor, SenderRequest> context;    

    private int receivedMessages;

    public NobodySenderActor(IActorContext<NobodySenderActor, SenderRequest> context)
    {
        this.context = context;
    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    public void IncrMessage()
    {
        receivedMessages++;
    }

    public async Task Receive(SenderRequest message)
    {
        await Task.Yield();

        if (context.Sender is IActorRef<NobodyActor, object> nobodyActor)
        {
            IncrMessage();
            nobodyActor.Send(message);
        }
    }
}