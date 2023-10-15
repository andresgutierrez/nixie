
using Nixie.Tests.Actors;

namespace Nixie.Tests;

public sealed class TestBasics
{
    [Fact]
    public void TestCreateFireAndForgetActor()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Create<FireAndForgetActor, string>();
      
        Assert.IsAssignableFrom<FireAndForgetActor>(actor.Runner.Actor);
    }

    [Fact]
    public void TestCreateReplyActor()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Create<ReplyActor, string, string>();

        Assert.IsAssignableFrom<ReplyActor>(actor.Runner.Actor);
    }

    [Fact]
    public void TestCreateFireAndForgetActorAndGet()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Create<FireAndForgetActor, string>("my-actor");

        Assert.IsAssignableFrom<FireAndForgetActor>(actor.Runner.Actor);

        IActorRef<FireAndForgetActor, string>? actor2 = asx.Get<FireAndForgetActor, string>("my-actor");
        Assert.NotNull(actor2);

        Assert.Equal(actor, actor2);
    }

    [Fact]
    public void TestCreateFireAndForgetActorAndGet2()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string>? notExistActor = asx.Get<FireAndForgetActor, string>("my-actor");
        Assert.Null(notExistActor);

        IActorRef<FireAndForgetActor, string> actor = asx.Create<FireAndForgetActor, string>("my-actor");

        Assert.IsAssignableFrom<FireAndForgetActor>(actor.Runner.Actor);

        IActorRef<FireAndForgetActor, string>? actor2 = asx.Get<FireAndForgetActor, string>("my-actor");
        Assert.NotNull(actor2);

        Assert.Equal(actor, actor2);
    }

    [Fact]
    public void TestCreateReplyActorAndGet()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Create<ReplyActor, string, string>("my-actor");

        Assert.IsAssignableFrom<ReplyActor>(actor.Runner.Actor);

        IActorRef<ReplyActor, string, string>? actor2 = asx.Get<ReplyActor, string, string>("my-actor");
        Assert.NotNull(actor2);

        Assert.Equal(actor, actor2);
    }

    [Fact]
    public void TestCreateReplyActorAndGet2()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string>? notExists = asx.Get<ReplyActor, string, string>("my-actor");
        Assert.Null(notExists);

        IActorRef<ReplyActor, string, string> actor = asx.Create<ReplyActor, string, string>("my-actor");

        Assert.IsAssignableFrom<ReplyActor>(actor.Runner.Actor);

        IActorRef<ReplyActor, string, string>? actor2 = asx.Get<ReplyActor, string, string>("my-actor");
        Assert.NotNull(actor2);

        Assert.Equal(actor, actor2);
    }
}