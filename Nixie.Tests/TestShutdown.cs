
using Nixie.Tests.Actors;

namespace Nixie.Tests;

[Collection("Nixie")]
public sealed class TestShutdown
{
    [Fact]
    public async Task TestSpawnFireAndForgetActorAndShutdownByName()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownActor, string> actor = asx.Spawn<ShutdownActor, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName");

        await asx.Wait();

        Assert.True(asx.Shutdown<ShutdownActor, string>("my-actor"));

        await asx.Wait();

        Assert.Equal(1, ((ShutdownActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownActor, string>? actor2 = asx.Get<ShutdownActor, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetActorAndShutdownByName2()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownActor, string> actor = asx.Spawn<ShutdownActor, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName2");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName2");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName2");

        await asx.Wait();

        Assert.True(asx.Shutdown<ShutdownActor, string>("my-actor"));

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName2");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName2");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName2");

        await asx.Wait();

        Assert.Equal(3, ((ShutdownActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownActor, string>? actor2 = asx.Get<ShutdownActor, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetActorAndShutdownByRef()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownActor, string> actor = asx.Spawn<ShutdownActor, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByRef");

        await asx.Wait();

        Assert.True(asx.Shutdown(actor));

        await asx.Wait();

        Assert.Equal(1, ((ShutdownActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownActor, string>? actor2 = asx.Get<ShutdownActor, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetActorAndShutdownByRef2()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownActor, string> actor = asx.Spawn<ShutdownActor, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByRef2");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByRef2");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByRef2");

        await asx.Wait();

        Assert.True(asx.Shutdown(actor));

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByRef2");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByRef2");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByRef2");

        await asx.Wait();

        Assert.Equal(3, ((ShutdownActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownActor, string>? actor2 = asx.Get<ShutdownActor, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnActorAndShutdownInside()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownInsideActor, string> actor = asx.Spawn<ShutdownInsideActor, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownInsideActor>(actor.Runner.Actor);

        actor.Send("TestSpawnActorAndShutdownInside");

        await asx.Wait();

        actor.Send("shutdown");

        await asx.Wait();

        Assert.Equal(1, ((ShutdownInsideActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownInsideActor, string>? actor2 = asx.Get<ShutdownInsideActor, string>("my-actor");
        Assert.Null(actor2);
    }
}
