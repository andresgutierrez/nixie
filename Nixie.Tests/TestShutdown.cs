
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
    public async Task TestSpawnReplyActorAndShutdownByName()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownReplyActor, string, string> actor = asx.Spawn<ShutdownReplyActor, string, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownReplyActor>(actor.Runner.Actor);

        actor.Send("TestSpawnReplyActorAndShutdownByName");

        await asx.Wait();

        Assert.True(asx.Shutdown<ShutdownReplyActor, string, string>("my-actor"));

        await asx.Wait();

        Assert.Equal(1, ((ShutdownReplyActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownReplyActor, string, string>? actor2 = asx.Get<ShutdownReplyActor, string, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetActorStructAndShutdownByName()
    {
        using ActorSystem asx = new();

        IActorRefStruct<ShutdownActorStruct, int> actor = asx.SpawnStruct<ShutdownActorStruct, int>("my-actor");

        Assert.IsAssignableFrom<ShutdownActorStruct>(actor.Runner.Actor);

        actor.Send(100);

        await asx.Wait();

        Assert.True(asx.ShutdownStruct<ShutdownActorStruct, int>("my-actor"));

        await asx.Wait();

        Assert.Equal(1, ((ShutdownActorStruct)actor.Runner.Actor!).GetMessages());

        IActorRefStruct<ShutdownActorStruct, int>? actor2 = asx.GetStruct<ShutdownActorStruct, int>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetActorAndPostShutdownByName()
    {
        using ActorSystem asx = new();

        IActorRef<PostShutdownActor, string> actor = asx.Spawn<PostShutdownActor, string>("my-actor");

        Assert.IsAssignableFrom<PostShutdownActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndPostShutdownByName");

        await asx.Wait();

        Assert.True(asx.Shutdown<PostShutdownActor, string>("my-actor"));

        await asx.Wait();

        Assert.Equal(1, ((PostShutdownActor)actor.Runner.Actor!).GetMessages());
        Assert.True(((PostShutdownActor)actor.Runner.Actor!).GetShutdownFlag());

        IActorRef<PostShutdownActor, string>? actor2 = asx.Get<PostShutdownActor, string>("my-actor");
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
    public async Task TestSpawnReplyActorAndShutdownByName2()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownReplyActor, string, string> actor = asx.Spawn<ShutdownReplyActor, string, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownReplyActor>(actor.Runner.Actor);

        actor.Send("TestSpawnReplyActorAndShutdownByName2");
        actor.Send("TestSpawnReplyActorAndShutdownByName2");
        actor.Send("TestSpawnReplyActorAndShutdownByName2");

        await asx.Wait();

        Assert.True(asx.Shutdown<ShutdownReplyActor, string, string>("my-actor"));

        actor.Send("TestSpawnReplyActorAndShutdownByName2");
        actor.Send("TestSpawnReplyActorAndShutdownByName2");
        actor.Send("TestSpawnReplyActorAndShutdownByName2");

        await asx.Wait();

        Assert.Equal(3, ((ShutdownReplyActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownReplyActor, string, string>? actor2 = asx.Get<ShutdownReplyActor, string, string>("my-actor");
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
    public async Task TestSpawnReplyActorAndShutdownByRef()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownReplyActor, string, string> actor = asx.Spawn<ShutdownReplyActor, string, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownReplyActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByRef");

        await asx.Wait();

        Assert.True(asx.Shutdown(actor));

        await asx.Wait();

        Assert.Equal(1, ((ShutdownReplyActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownReplyActor, string, string>? actor2 = asx.Get<ShutdownReplyActor, string, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetActorStructAndShutdownByRef()
    {
        using ActorSystem asx = new();

        IActorRefStruct<ShutdownActorStruct, int> actor = asx.SpawnStruct<ShutdownActorStruct, int>("my-actor");

        Assert.IsAssignableFrom<ShutdownActorStruct>(actor.Runner.Actor);

        actor.Send(100);

        await asx.Wait();

        Assert.True(asx.ShutdownStruct(actor));

        await asx.Wait();

        Assert.Equal(1, ((ShutdownActorStruct)actor.Runner.Actor!).GetMessages());

        IActorRefStruct<ShutdownActorStruct, int>? actor2 = asx.GetStruct<ShutdownActorStruct, int>("my-actor");
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
    public async Task TestSpawnReplyActorAndShutdownByRef2()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownReplyActor, string, string> actor = asx.Spawn<ShutdownReplyActor, string, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownReplyActor>(actor.Runner.Actor);

        actor.Send("TestSpawnReplyAndShutdownByRef2");
        actor.Send("TestSpawnReplyAndShutdownByRef2");
        actor.Send("TestSpawnReplyAndShutdownByRef2");

        await asx.Wait();

        Assert.True(asx.Shutdown(actor));

        actor.Send("TestSpawnReplyAndShutdownByRef2");
        actor.Send("TestSpawnReplyAndShutdownByRef2");
        actor.Send("TestSpawnReplyAndShutdownByRef2");

        await asx.Wait();

        Assert.Equal(3, ((ShutdownReplyActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownReplyActor, string, string>? actor2 = asx.Get<ShutdownReplyActor, string, string>("my-actor");
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

    [Fact]
    public async Task TestSpawnFireAndForgetActorAndGracefulShutdownByName()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownActor, string> actor = asx.Spawn<ShutdownActor, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName");

        await asx.Wait();

        Assert.True(await asx.GracefulShutdown<ShutdownActor, string>("my-actor", TimeSpan.FromMinutes(1)));

        await asx.Wait();

        Assert.Equal(1, ((ShutdownActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownActor, string>? actor2 = asx.Get<ShutdownActor, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetActorAndScheduleShutdownByRef()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownActor, string> actor = asx.Spawn<ShutdownActor, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName");

        await asx.Wait();

        Assert.Equal(1, ((ShutdownActor)actor.Runner.Actor!).GetMessages());

        asx.ScheduleShutdown(actor, TimeSpan.FromMilliseconds(1000));

        await Task.Delay(2000);

        await asx.Wait();

        IActorRef<ShutdownActor, string>? actor2 = asx.Get<ShutdownActor, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetSlowActorAndGracefulShutdownByName()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownSlowActor, string> actor = asx.Spawn<ShutdownSlowActor, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownSlowActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName");

        await asx.Wait();

        Assert.True(await asx.GracefulShutdown<ShutdownSlowActor, string>("my-actor", TimeSpan.FromMinutes(1)));

        await asx.Wait();

        Assert.Equal(1, ((ShutdownSlowActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownSlowActor, string>? actor2 = asx.Get<ShutdownSlowActor, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetSlowActorAndGracefulShutdownShortDelayByName()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownSlowActor, string> actor = asx.Spawn<ShutdownSlowActor, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownSlowActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName");

        Assert.False(await asx.GracefulShutdown<ShutdownSlowActor, string>("my-actor", TimeSpan.FromMilliseconds(100)));

        await asx.Wait();

        Assert.Equal(0, ((ShutdownSlowActor)actor.Runner.Actor!).GetMessages());

        IActorRef<ShutdownSlowActor, string>? actor2 = asx.Get<ShutdownSlowActor, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetSlowActorAndGracefulShutdownShortDelayByName2()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownSlowActor, string> actor = asx.Spawn<ShutdownSlowActor, string>("my-actor");

        Assert.IsAssignableFrom<ShutdownSlowActor>(actor.Runner.Actor);

        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName2");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName2");
        actor.Send("TestSpawnFireAndForgetActorAndShutdownByName2");

        Assert.False(await asx.GracefulShutdown<ShutdownSlowActor, string>("my-actor", TimeSpan.FromMilliseconds(2000)));

        await asx.Wait();

        int x = ((ShutdownSlowActor)actor.Runner.Actor!).GetMessages();
        Assert.True(2 == x || x == 1);

        IActorRef<ShutdownSlowActor, string>? actor2 = asx.Get<ShutdownSlowActor, string>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestSpawnFireAndForgetActorStructAndGracefulShutdownByName()
    {
        using ActorSystem asx = new();

        IActorRefStruct<ShutdownActorStruct, int> actor = asx.SpawnStruct<ShutdownActorStruct, int>("my-actor");

        Assert.IsAssignableFrom<ShutdownActorStruct>(actor.Runner.Actor);

        actor.Send(100);

        await asx.Wait();

        Assert.True(await asx.GracefulShutdownStruct<ShutdownActorStruct, int>("my-actor", TimeSpan.FromMinutes(1)));

        await asx.Wait();

        Assert.Equal(1, ((ShutdownActorStruct)actor.Runner.Actor!).GetMessages());

        IActorRefStruct<ShutdownActorStruct, int>? actor2 = asx.GetStruct<ShutdownActorStruct, int>("my-actor");
        Assert.Null(actor2);
    }

    [Fact]
    public async Task TestGracefulShutdownAllRemovesActorsFromAllRepositories()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownActor, string> actor = asx.Spawn<ShutdownActor, string>("shutdown-actor");
        IActorRef<ShutdownReplyActor, string, string> replyActor = asx.Spawn<ShutdownReplyActor, string, string>("shutdown-reply-actor");
        IActorRefStruct<ShutdownActorStruct, int> structActor = asx.SpawnStruct<ShutdownActorStruct, int>("shutdown-struct-actor");
        IActorRefStruct<ReplyActorStruct, int, int> structReplyActor = asx.SpawnStruct<ReplyActorStruct, int, int>("shutdown-struct-reply-actor");
        IActorRefAggregate<FireAndForgetAggregateActor, string> aggregateActor = asx.SpawnAggregate<FireAndForgetAggregateActor, string>("shutdown-aggregate-actor");
        IActorRefAggregate<ReplyAggregateActor, string, string> aggregateReplyActor = asx.SpawnAggregate<ReplyAggregateActor, string, string>("shutdown-aggregate-reply-actor");

        actor.Send("message");
        replyActor.Send("message");
        structActor.Send(1);
        structReplyActor.Send(1);
        aggregateActor.Send("message");
        aggregateReplyActor.Send("message");

        await asx.Wait();

        await asx.GracefulShutdownAll(TimeSpan.FromSeconds(5));

        Assert.Equal(1, ((ShutdownActor)actor.Runner.Actor!).GetMessages());
        Assert.Equal(1, ((ShutdownReplyActor)replyActor.Runner.Actor!).GetMessages());
        Assert.Equal(1, ((ShutdownActorStruct)structActor.Runner.Actor!).GetMessages());
        Assert.Equal(1, ((ReplyActorStruct)structReplyActor.Runner.Actor!).GetMessages(1));
        Assert.Equal(1, ((FireAndForgetAggregateActor)aggregateActor.Runner.Actor!).GetMessages("message"));
        Assert.Equal(1, ((ReplyAggregateActor)aggregateReplyActor.Runner.Actor!).GetMessages("message"));

        Assert.Null(asx.Get<ShutdownActor, string>("shutdown-actor"));
        Assert.Null(asx.Get<ShutdownReplyActor, string, string>("shutdown-reply-actor"));
        Assert.Null(asx.GetStruct<ShutdownActorStruct, int>("shutdown-struct-actor"));
        Assert.Null(asx.GetRepositoryStruct<ReplyActorStruct, int, int>().Get("shutdown-struct-reply-actor"));
        Assert.Null(asx.GetRepositoryAggregate<FireAndForgetAggregateActor, string>().Get("shutdown-aggregate-actor"));
        Assert.Null(asx.GetRepositoryAggregate<ReplyAggregateActor, string, string>().Get("shutdown-aggregate-reply-actor"));
    }

    [Fact]
    public async Task TestGracefulShutdownAllWaitsForQueuedMessages()
    {
        using ActorSystem asx = new();

        IActorRef<ShutdownSlowActor, string> actor = asx.Spawn<ShutdownSlowActor, string>("slow-actor");

        actor.Send("message");
        actor.Send("message");
        actor.Send("message");

        await asx.GracefulShutdownAll(TimeSpan.FromSeconds(5));

        Assert.Equal(3, ((ShutdownSlowActor)actor.Runner.Actor!).GetMessages());
        Assert.Null(asx.Get<ShutdownSlowActor, string>("slow-actor"));
    }
}
