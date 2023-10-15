
using Nixie.Tests.Actors;

namespace Nixie.Tests;

public sealed class TestBasics
{
    [Fact]
    public void TestSpawnFireAndForgetActor()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Spawn<FireAndForgetActor, string>();

        Assert.IsAssignableFrom<FireAndForgetActor>(actor.Runner.Actor);
    }

    [Fact]
    public void TestSpawnReplyActor()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Spawn<ReplyActor, string, string>();

        Assert.IsAssignableFrom<ReplyActor>(actor.Runner.Actor);
    }

    [Fact]
    public void TestSpawnFireAndForgetActorAndGet()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Spawn<FireAndForgetActor, string>("my-actor");

        Assert.IsAssignableFrom<FireAndForgetActor>(actor.Runner.Actor);

        IActorRef<FireAndForgetActor, string>? actor2 = asx.Get<FireAndForgetActor, string>("my-actor");
        Assert.NotNull(actor2);

        Assert.Equal(actor, actor2);
    }

    [Fact]
    public void TestSpawnFireAndForgetActorAndGet2()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string>? notExistActor = asx.Get<FireAndForgetActor, string>("my-actor");
        Assert.Null(notExistActor);

        IActorRef<FireAndForgetActor, string> actor = asx.Spawn<FireAndForgetActor, string>("my-actor");

        Assert.IsAssignableFrom<FireAndForgetActor>(actor.Runner.Actor);

        IActorRef<FireAndForgetActor, string>? actor2 = asx.Get<FireAndForgetActor, string>("my-actor");
        Assert.NotNull(actor2);

        Assert.Equal(actor, actor2);
    }

    [Fact]
    public void TestSpawnReplyActorAndGet()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Spawn<ReplyActor, string, string>("my-actor");

        Assert.IsAssignableFrom<ReplyActor>(actor.Runner.Actor);

        IActorRef<ReplyActor, string, string>? actor2 = asx.Get<ReplyActor, string, string>("my-actor");
        Assert.NotNull(actor2);

        Assert.Equal(actor, actor2);
    }

    [Fact]
    public void TestSpawnReplyActorAndGet2()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string>? notExists = asx.Get<ReplyActor, string, string>("my-actor");
        Assert.Null(notExists);

        IActorRef<ReplyActor, string, string> actor = asx.Spawn<ReplyActor, string, string>("my-actor");

        Assert.IsAssignableFrom<ReplyActor>(actor.Runner.Actor);

        IActorRef<ReplyActor, string, string>? actor2 = asx.Get<ReplyActor, string, string>("my-actor");
        Assert.NotNull(actor2);

        Assert.Equal(actor, actor2);
    }

    [Fact]
    public void TestSpawnActorWithSameNameTwice()
    {
        using ActorSystem asx = new();

        NixieException exception = Assert.Throws<NixieException>(() =>
        {
            asx.Spawn<PeriodicTimerActor, string>("same-name");
            asx.Spawn<PeriodicTimerActor, string>("same-name");
        });

        Assert.Equal("Actor already exists", exception.Message);
    }

    [Fact]
    public void TestSpawnActorWithSameNameTwiceButDifferentSystem()
    {
        using ActorSystem asx1 = new();

        IActorRef<FireAndForgetActor, string> actor1 = asx1.Spawn<FireAndForgetActor, string>("same-name");

        Assert.IsAssignableFrom<FireAndForgetActor>(actor1.Runner.Actor);

        IActorRef<FireAndForgetActor, string>? actor4 = asx1.Get<FireAndForgetActor, string>("same-name");
        Assert.NotNull(actor4);

        Assert.Equal(actor1, actor4);

        using ActorSystem asx2 = new();

        IActorRef<FireAndForgetActor, string> actor2 = asx2.Spawn<FireAndForgetActor, string>("same-name");

        Assert.IsAssignableFrom<FireAndForgetActor>(actor2.Runner.Actor);

        IActorRef<FireAndForgetActor, string>? actor3 = asx2.Get<FireAndForgetActor, string>("same-name");
        Assert.NotNull(actor3);

        Assert.Equal(actor2, actor3);
    }

    [Fact]
    public void TestSpawnFireAndForgetActorProps()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetPropsActor, string> actor = asx.Spawn<FireAndForgetPropsActor, string>(null, new FireAndForgetPropsSettings());

        Assert.IsAssignableFrom<FireAndForgetPropsActor>(actor.Runner.Actor);
    }
}