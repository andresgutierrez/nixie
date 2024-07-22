
using System.Diagnostics;
using Nixie.Tests.Actors;

namespace Nixie.Tests;

[Collection("Nixie")]
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

    [Fact]
    public async Task TestSkynet()
    {
        using ActorSystem asx = new();

        IActorRef<SenderActor, SenderRequest> actor = asx.Spawn<SenderActor, SenderRequest>();

        Stopwatch st = Stopwatch.StartNew();

        SenderRequest request = new(SenderRequestType.Broadcast);

        for (int i = 0; i < 100000; i++)
            actor.Send(request);

        await asx.Wait();

        Console.WriteLine("Time=", st.ElapsedMilliseconds);

        Assert.Equal(1000000, ((SenderActor)actor.Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestBroadcastMessageAndUseSenderToReplyStruct()
    {
        using ActorSystem asx = new();

        IActorRefStruct<SenderActorStruct, SenderRequestStruct> actor = asx.SpawnStruct<SenderActorStruct, SenderRequestStruct>();

        actor.Send(new SenderRequestStruct(SenderRequestType.Broadcast));

        await asx.Wait();

        Assert.Equal(10, ((SenderActorStruct)actor.Runner.Actor!).GetMessages());
    }
}