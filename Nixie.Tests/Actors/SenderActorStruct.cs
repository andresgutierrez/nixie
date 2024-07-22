
using Nixie.Actors;

namespace Nixie.Tests.Actors;

internal readonly struct SenderRequestStruct
{
    public SenderRequestType Type { get; }

    public SenderRequestStruct(SenderRequestType type)
    {
        Type = type;
    }
}

internal sealed class SenderPongActorStruct : IActorStruct<SenderRequestStruct>
{
    private readonly IActorContextStruct<SenderPongActorStruct, SenderRequestStruct> context;

    public SenderPongActorStruct(IActorContextStruct<SenderPongActorStruct, SenderRequestStruct> context)
    {
        this.context = context;
    }

    public Task Receive(SenderRequestStruct message)
    {
        if (message.Type != SenderRequestType.Broadcast)
            return Task.CompletedTask;

        if (context.Sender is IActorRefStruct<SenderActorStruct, SenderRequestStruct> senderActor)
            senderActor.Send(new SenderRequestStruct(SenderRequestType.Pong));

        if (context.Sender is IActorRef<NobodyActor, object> nobodyActor)
            nobodyActor.Send(new SenderRequest(SenderRequestType.Pong));
        
        return Task.CompletedTask;
    }
}

internal sealed class SenderActorStruct : IActorStruct<SenderRequestStruct>
{
    private readonly IActorContextStruct<SenderActorStruct, SenderRequestStruct> context;

    private readonly List<IActorRefStruct<SenderPongActorStruct, SenderRequestStruct>> senderPongRefs = new();

    private int receivedMessages;

    public SenderActorStruct(IActorContextStruct<SenderActorStruct, SenderRequestStruct> context)
    {
        this.context = context;

        for (int i = 0; i < 10; i++)
            senderPongRefs.Add(context.ActorSystem.SpawnStruct<SenderPongActorStruct, SenderRequestStruct>());
    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    public void IncrMessage()
    {
        receivedMessages++;
    }

    public Task Receive(SenderRequestStruct message)
    {
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

        return Task.CompletedTask;
    }
}