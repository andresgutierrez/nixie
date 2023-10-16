
using Nixie.Tests.Actors;

namespace Nixie.Tests;

public class TestSender
{
    [Fact]
    public async Task TestBroadcastMessageAndUseSenderToReply()
    {
        using ActorSystem asx = new();

        IActorRef<SenderActor, SenderRequest> actor = asx.Spawn<SenderActor, SenderRequest>();

        actor.Send(new SenderRequest(SenderRequestType.Broadcast));

        await asx.Wait();

        Assert.Equal(10, ((SenderActor)actor.Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestBroadcastMultipleMessageAndUseSenderToReply()
    {
        using ActorSystem asx = new();

        IActorRef<SenderActor, SenderRequest> actor = asx.Spawn<SenderActor, SenderRequest>();

        for (int i = 0; i < 10; i++)
            actor.Send(new SenderRequest(SenderRequestType.Broadcast));

        await asx.Wait();

        Assert.Equal(100, ((SenderActor)actor.Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestBroadcastFromNobodySenderActor()
    {
        using ActorSystem asx = new();

        IActorRef<NobodySenderActor, SenderRequest> actor = asx.Spawn<NobodySenderActor, SenderRequest>();

        actor.Send(new SenderRequest(SenderRequestType.Broadcast));

        await asx.Wait();

        Assert.Equal(1, ((NobodySenderActor)actor.Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestBroadcastMultipleFromNobodySenderActor()
    {
        using ActorSystem asx = new();

        IActorRef<NobodySenderActor, SenderRequest> actor = asx.Spawn<NobodySenderActor, SenderRequest>();

        for (int i = 0; i < 10; i++)
            actor.Send(new SenderRequest(SenderRequestType.Broadcast));

        await asx.Wait();

        Assert.Equal(10, ((NobodySenderActor)actor.Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestBroadcastMixedMessagesAndUseSenderToReply()
    {
        using ActorSystem asx = new();

        IActorRef<SenderActor, SenderRequest> actor = asx.Spawn<SenderActor, SenderRequest>();

        for (int i = 0; i < 10; i++)
        {
            actor.Send(new SenderRequest(SenderRequestType.Broadcast));
            actor.Send(new SenderRequest(SenderRequestType.BroadcastNobody));
        }

        await asx.Wait();

        Assert.Equal(100, ((SenderActor)actor.Runner.Actor!).GetMessages());
    }
}