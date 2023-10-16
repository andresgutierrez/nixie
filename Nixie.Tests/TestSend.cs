﻿
using Nixie.Tests.Actors;

namespace Nixie.Tests;

public class TestSend
{
    [Fact]
    public async Task TestSendMessageToSingleActor()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Spawn<ReplyActor, string, string>();

        actor.Send("TestSendMessageToSingleActor");

        await asx.Wait();

        Assert.Equal(1, ((ReplyActor)actor.Runner.Actor!).GetMessages("TestSendMessageToSingleActor"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActor()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Spawn<ReplyActor, string, string>();

        actor.Send("TestSendMultipleMessageToSingleActor");
        actor.Send("TestSendMultipleMessageToSingleActor");
        actor.Send("TestSendMultipleMessageToSingleActor");

        await asx.Wait();

        Assert.Equal(3, ((ReplyActor)actor.Runner.Actor!).GetMessages("TestSendMultipleMessageToSingleActor"));
    }

    [Fact]
    public async Task TestSendMessageToSingleActorNoResponse()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Spawn<FireAndForgetActor, string>();

        actor.Send("TestSendMessageToSingleActorNoResponse");

        await asx.Wait();

        Assert.Equal(1, ((FireAndForgetActor)actor.Runner.Actor!).GetMessages("TestSendMessageToSingleActorNoResponse"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponse()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Spawn<FireAndForgetActor, string>("TestSendMultipleMessageToSingleActorNoResponse");

        actor.Send("TestSendMultipleMessageToSingleActorNoResponse");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponse");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponse");

        await asx.Wait();

        Assert.Equal(3, ((FireAndForgetActor)actor.Runner.Actor!).GetMessages("TestSendMultipleMessageToSingleActorNoResponse"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponse2()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Spawn<FireAndForgetActor, string>("TestSendMultipleMessageToSingleActorNoResponse");

        for (int i = 0; i < 100; i++)
            actor.Send("TestSendMultipleMessageToSingleActorNoResponse");

        await asx.Wait();

        Assert.Equal(100, ((FireAndForgetActor)actor.Runner.Actor!).GetMessages("TestSendMultipleMessageToSingleActorNoResponse"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponseSlow()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetSlowActor, string> actor = asx.Spawn<FireAndForgetSlowActor, string>("TestSendMultipleMessageToSingleActorNoResponseSlow");

        actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");

        await asx.Wait();

        Assert.Equal(3, ((FireAndForgetSlowActor)actor.Runner.Actor!).GetMessages("TestSendMultipleMessageToSingleActorNoResponseSlow"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponseSlow2()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetSlowActor, string> actor = asx.Spawn<FireAndForgetSlowActor, string>("TestSendMultipleMessageToSingleActorNoResponseSlow");

        for (int i = 0; i < 10; i++)
            actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");

        await asx.Wait();

        Assert.Equal(10, ((FireAndForgetSlowActor)actor.Runner.Actor!).GetMessages("TestSendMultipleMessageToSingleActorNoResponseSlow"));
    }

    [Fact]
    public async Task TestCreateMultipleActorsAndSendOneMessage()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string>[] actorRefs = new IActorRef<ReplyActor, string, string>[10];

        for (int i = 0; i < 10; i++)
            actorRefs[i] = asx.Spawn<ReplyActor, string, string>();

        for (int i = 0; i < 10; i++)
            actorRefs[i].Send("TestCreateMultipleActorsAndSendOneMessage");

        await asx.Wait();

        for (int i = 0; i < 10; i++)
            Assert.Equal(1, ((ReplyActor)actorRefs[i].Runner.Actor!).GetMessages("TestCreateMultipleActorsAndSendOneMessage"));
    }

    [Fact]
    public async Task TestCreateMultipleActorsAndSendOneMessage2()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string>[] actorRefs = new IActorRef<ReplyActor, string, string>[100];

        for (int i = 0; i < 100; i++)
            actorRefs[i] = asx.Spawn<ReplyActor, string, string>();

        for (int i = 0; i < 100; i++)
            actorRefs[i].Send("TestCreateMultipleActorsAndSendOneMessage");

        await asx.Wait();

        for (int i = 0; i < 100; i++)
            Assert.Equal(1, ((ReplyActor)actorRefs[i].Runner.Actor!).GetMessages("TestCreateMultipleActorsAndSendOneMessage"));
    }

    [Fact]
    public async Task TestSendMessageToFaultyActor()
    {
        using ActorSystem asx = new();

        IActorRef<FaultyActor, FaultyMessage> actor = asx.Spawn<FaultyActor, FaultyMessage>();

        actor.Send(new FaultyMessage(FaultyMessageType.Ok));
        actor.Send(new FaultyMessage(FaultyMessageType.Faulty));
        actor.Send(new FaultyMessage(FaultyMessageType.Ok));
        actor.Send(new FaultyMessage(FaultyMessageType.Ok));
        actor.Send(new FaultyMessage(FaultyMessageType.Faulty));

        await asx.Wait();

        Assert.Equal(3, ((FaultyActor)actor.Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestSpawnFireAndForgetActorProps()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetPropsActor, string> actor = asx.Spawn<FireAndForgetPropsActor, string>(null, new FireAndForgetPropsSettings());

        Assert.IsAssignableFrom<FireAndForgetPropsActor>(actor.Runner.Actor);

        actor.Send("TestSendMessageToSingleActor");

        await asx.Wait();

        Assert.Equal(1, ((FireAndForgetPropsActor)actor.Runner.Actor!).GetMessages("hello"));
    }

    [Fact]
    public async Task TestSpawnFireAndForgetActorPropsMultiple()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetPropsActor, string> actor = asx.Spawn<FireAndForgetPropsActor, string>(null, new FireAndForgetPropsSettings());

        Assert.IsAssignableFrom<FireAndForgetPropsActor>(actor.Runner.Actor);

        actor.Send("TestSendMessageToSingleActor");
        actor.Send("TestSendMessageToSingleActor");
        actor.Send("TestSendMessageToSingleActor");
        actor.Send("TestSendMessageToSingleActor");

        await asx.Wait();

        Assert.Equal(4, ((FireAndForgetPropsActor)actor.Runner.Actor!).GetMessages("hello"));
    }
}
