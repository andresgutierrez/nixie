
using Nixie.Actors;

namespace Nixie.Tests.Actors;

internal enum SenderRequestType
{
    Broadcast = 0,
    BroadcastNobody = 1,
    Pong = 2,
}

internal sealed class SenderRequest
{
    public SenderRequestType Type { get; }

    public SenderRequest(SenderRequestType type)
    {
        Type = type;
    }
}

internal sealed class SenderPongActor : IActor<SenderRequest>
{
    private readonly IActorContext<SenderPongActor, SenderRequest> context;

    public SenderPongActor(IActorContext<SenderPongActor, SenderRequest> context)
    {
        this.context = context;
    }

    public async Task Receive(SenderRequest message)
    {
        await Task.Yield();

        if (message.Type != SenderRequestType.Broadcast)
            return;

        if (context.Sender is IActorRef<SenderActor, SenderRequest> senderActor)
            senderActor.Send(new SenderRequest(SenderRequestType.Pong));

        if (context.Sender is IActorRef<NobodyActor, object> nobodyActor)
            nobodyActor.Send(new SenderRequest(SenderRequestType.Pong));
    }
}

internal sealed class SenderActor : IActor<SenderRequest>
{
    private readonly IActorContext<SenderActor, SenderRequest> context;

    private readonly List<IActorRef<SenderPongActor, SenderRequest>> senderPongRefs = new();

    private int receivedMessages;

    public SenderActor(IActorContext<SenderActor, SenderRequest> context)
    {
        this.context = context;

        for (int i = 0; i < 10; i++)
            senderPongRefs.Add(context.ActorSystem.Spawn<SenderPongActor, SenderRequest>());
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

        if (message.Type == SenderRequestType.Broadcast)
        {
            for (int i = 0; i < 10; i++)
                senderPongRefs[i].Send(message, context.Self);
        }

        if (message.Type == SenderRequestType.BroadcastNobody)
        {
            for (int i = 0; i < 10; i++)
                senderPongRefs[i].Send(message);
        }

        if (message.Type == SenderRequestType.Pong)
            IncrMessage();
    }
}