
using Nyx.Tests.Actors;

namespace Nyx.Tests;

public class TestSend
{
    [Fact]
    public async Task TestSendMessageToSingleActor()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Create<ReplyActor, string, string>();

        actor.Send("TestSendMessageToSingleActor");

        await asx.Wait();

        Assert.Equal(1, ((ReplyActor)actor.Runner.Actor).GetMessages("TestSendMessageToSingleActor"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActor()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Create<ReplyActor, string, string>();

        actor.Send("TestSendMultipleMessageToSingleActor");
        actor.Send("TestSendMultipleMessageToSingleActor");
        actor.Send("TestSendMultipleMessageToSingleActor");

        await asx.Wait();

        Assert.Equal(3, ((ReplyActor)actor.Runner.Actor).GetMessages("TestSendMultipleMessageToSingleActor"));
    }

    [Fact]
    public async Task TestSendMessageToSingleActorNoResponse()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Create<FireAndForgetActor, string>();

        actor.Send("TestSendMessageToSingleActorNoResponse");

        await asx.Wait();

        Assert.Equal(1, ((FireAndForgetActor)actor.Runner.Actor).GetMessages("TestSendMessageToSingleActorNoResponse"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponse()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Create<FireAndForgetActor, string>("TestSendMultipleMessageToSingleActorNoResponse");

        actor.Send("TestSendMultipleMessageToSingleActorNoResponse");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponse");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponse");

        await asx.Wait();

        Assert.Equal(3, ((FireAndForgetActor)actor.Runner.Actor).GetMessages("TestSendMultipleMessageToSingleActorNoResponse"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponse2()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Create<FireAndForgetActor, string>("TestSendMultipleMessageToSingleActorNoResponse");

        for (int i = 0; i < 100; i++)
            actor.Send("TestSendMultipleMessageToSingleActorNoResponse");

        await asx.Wait();

        Assert.Equal(100, ((FireAndForgetActor)actor.Runner.Actor).GetMessages("TestSendMultipleMessageToSingleActorNoResponse"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponseSlow()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetSlowActor, string> actor = asx.Create<FireAndForgetSlowActor, string>("TestSendMultipleMessageToSingleActorNoResponseSlow");

        actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");

        await asx.Wait();

        Assert.Equal(3, ((FireAndForgetSlowActor)actor.Runner.Actor).GetMessages("TestSendMultipleMessageToSingleActorNoResponseSlow"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponseSlow2()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetSlowActor, string> actor = asx.Create<FireAndForgetSlowActor, string>("TestSendMultipleMessageToSingleActorNoResponseSlow");

        for (int i = 0; i < 10; i++)
            actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");

        await asx.Wait();

        Assert.Equal(10, ((FireAndForgetSlowActor)actor.Runner.Actor).GetMessages("TestSendMultipleMessageToSingleActorNoResponseSlow"));
    }

    [Fact]
    public async Task TestCreateMultipleActorsAndSendOneMessage()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string>[] actorRefs = new IActorRef<ReplyActor, string, string>[10];

        for (int i = 0; i < 10; i++)
            actorRefs[i] = asx.Create<ReplyActor, string, string>();

        for (int i = 0; i < 10; i++)
            actorRefs[i].Send("TestCreateMultipleActorsAndSendOneMessage");

        await asx.Wait();

        for (int i = 0; i < 10; i++)
            Assert.Equal(1, ((ReplyActor)actorRefs[i].Runner.Actor).GetMessages("TestCreateMultipleActorsAndSendOneMessage"));
    }

    [Fact]
    public async Task TestCreateMultipleActorsAndSendOneMessage2()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string>[] actorRefs = new IActorRef<ReplyActor, string, string>[100];

        for (int i = 0; i < 100; i++)
            actorRefs[i] = asx.Create<ReplyActor, string, string>();

        for (int i = 0; i < 100; i++)
            actorRefs[i].Send("TestCreateMultipleActorsAndSendOneMessage");

        await asx.Wait();

        for (int i = 0; i < 100; i++)
            Assert.Equal(1, ((ReplyActor)actorRefs[i].Runner.Actor).GetMessages("TestCreateMultipleActorsAndSendOneMessage"));
    }
}
