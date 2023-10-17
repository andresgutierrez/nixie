
using Microsoft.Extensions.Logging;
using Nixie.Tests.Actors;

namespace Nixie.Tests;

[Collection("Nixie")]
public class TestAskReplies
{
    [Fact]
    public async Task TestAskMessageToSingleActor()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Spawn<ReplyActor, string, string>();

        string? reply = await actor.Ask("TestSendMessageToSingleActor");
        Assert.NotNull(reply);
        Assert.Equal("TestSendMessageToSingleActor", reply);

        Assert.Equal(1, ((ReplyActor)actor.Runner.Actor!).GetMessages("TestSendMessageToSingleActor"));
    }

    [Fact]
    public async Task TestAskMessageToSingleActorWithTimeout()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Spawn<ReplyActor, string, string>();

        string? reply = await actor.Ask("TestSendMessageToSingleActor", TimeSpan.FromSeconds(5));
        Assert.NotNull(reply);
        Assert.Equal("TestSendMessageToSingleActor", reply);

        Assert.Equal(1, ((ReplyActor)actor.Runner.Actor!).GetMessages("TestSendMessageToSingleActor"));
    }

    [Fact]
    public async Task TestCreateMultipleActorsAndAskOneMessage()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string>[] actorRefs = new IActorRef<ReplyActor, string, string>[10];

        for (int i = 0; i < 10; i++)
            actorRefs[i] = asx.Spawn<ReplyActor, string, string>();

        for (int i = 0; i < 10; i++)
        {
            string? response = await actorRefs[i].Ask("TestCreateMultipleActorsAndAskOneMessage");
            Assert.NotNull(response);
            Assert.Equal("TestCreateMultipleActorsAndAskOneMessage", response);
        }

        await asx.Wait();

        for (int i = 0; i < 10; i++)
            Assert.Equal(1, ((ReplyActor)actorRefs[i].Runner.Actor!).GetMessages("TestCreateMultipleActorsAndAskOneMessage"));
    }

    [Fact]
    public async Task TestCreateMultipleActorsAndAskOneMessageWithTimeout()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string>[] actorRefs = new IActorRef<ReplyActor, string, string>[10];

        for (int i = 0; i < 10; i++)
            actorRefs[i] = asx.Spawn<ReplyActor, string, string>();

        for (int i = 0; i < 10; i++)
        {
            string? response = await actorRefs[i].Ask("TestCreateMultipleActorsAndAskOneMessage", TimeSpan.FromSeconds(5));
            Assert.NotNull(response);
            Assert.Equal("TestCreateMultipleActorsAndAskOneMessage", response);
        }

        await asx.Wait();

        for (int i = 0; i < 10; i++)
            Assert.Equal(1, ((ReplyActor)actorRefs[i].Runner.Actor!).GetMessages("TestCreateMultipleActorsAndAskOneMessage"));
    }

    [Fact]
    public async Task TestCreateMultipleActorsAndAskOneMessage2()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string>[] actorRefs = new IActorRef<ReplyActor, string, string>[100];

        for (int i = 0; i < 100; i++)
            actorRefs[i] = asx.Spawn<ReplyActor, string, string>();

        for (int i = 0; i < 100; i++)
        {
            string expected = "TestCreateMultipleActorsAndAskOneMessage2" + i;

            string? response = await actorRefs[i].Ask(expected);
            Assert.NotNull(response);
            Assert.Equal(expected, response);
        }

        await asx.Wait();
    }

    [Fact]
    public async Task TestAskPingPong()
    {
        using ActorSystem asx = new();

        IActorRef<PingActor, string, string> actor = asx.Spawn<PingActor, string, string>();

        string? reply = await actor.Ask("TestAskPingPong");
        Assert.NotNull(reply);
        Assert.Equal("TestAskPingPong", reply);

        await asx.Wait();

        Assert.Equal(1, ((PingActor)actor.Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestCreateMultiplePingPingAndAsk()
    {
        using ActorSystem asx = new();

        IActorRef<PingActor, string, string>[] actorRefs = new IActorRef<PingActor, string, string>[100];

        for (int i = 0; i < 100; i++)
            actorRefs[i] = asx.Spawn<PingActor, string, string>();

        for (int i = 0; i < 100; i++)
        {
            string expected = "TestAskPingPong" + i;

            string? response = await actorRefs[i].Ask(expected);
            Assert.NotNull(response);
            Assert.Equal(expected, response);
        }

        await asx.Wait();

        for (int i = 0; i < 100; i++)
            Assert.Equal(1, ((PingActor)actorRefs[i].Runner.Actor!).GetMessages());
    }

    private async Task AskPingPong(IActorRef<PingActor, string, string> pingRef, int i)
    {
        string expected = "TestAskPingPong" + i;

        string? response = await pingRef.Ask(expected);
        Assert.NotNull(response);
        Assert.Equal(expected, response);
    }

    [Fact]
    public async Task TestCreateMultiplePingPingAndAskRace()
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Nixie", LogLevel.Debug)
                .AddConsole();
        });

        using ActorSystem asx = new(logger: loggerFactory.CreateLogger<TestSendMessages>());

        IActorRef<PingActor, string, string>[] actorRefs = new IActorRef<PingActor, string, string>[100];

        for (int i = 0; i < 100; i++)
            actorRefs[i] = asx.Spawn<PingActor, string, string>();

        Task[] tasks = new Task[100];

        for (int i = 0; i < 100; i++)
            tasks[i] = AskPingPong(actorRefs[i], i);

        await Task.WhenAll(tasks);

        await asx.Wait();

        for (int i = 0; i < 100; i++)
            Assert.Equal(1, ((PingActor)actorRefs[i].Runner.Actor!).GetMessages());
    }

    private async Task AskPingPongMultiple(IActorRef<PingActor, string, string> pingRef, int i)
    {
        string expected = "TestAskPingPong" + i;

        for (int j = 0; j < 50; j++)
        {
            string? response = await pingRef.Ask(expected);
            Assert.NotNull(response);
            Assert.Equal(expected, response);
        }
    }

    [Fact]
    public async Task TestCreateMultiplePingPingAndAskMultipleRace()
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Nixie", LogLevel.Debug)
                .AddConsole();
        });

        using ActorSystem asx = new(logger: loggerFactory.CreateLogger<TestSendMessages>());

        IActorRef<PingActor, string, string>[] actorRefs = new IActorRef<PingActor, string, string>[100];

        for (int i = 0; i < 100; i++)
            actorRefs[i] = asx.Spawn<PingActor, string, string>();

        Task[] tasks = new Task[100];

        for (int i = 0; i < 100; i++)
            tasks[i] = AskPingPongMultiple(actorRefs[i], i);

        await Task.WhenAll(tasks);

        await asx.Wait();

        for (int i = 0; i < 100; i++)
            Assert.Equal(50, ((PingActor)actorRefs[i].Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestAskMessageToSlowActor()
    {
        using ActorSystem asx = new();

        IActorRef<ReplySlowActor, string, string> actor = asx.Spawn<ReplySlowActor, string, string>();

        string? reply = await actor.Ask("TestAskMessageToSlowActor");
        Assert.NotNull(reply);
        Assert.Equal("TestAskMessageToSlowActor", reply);

        Assert.Equal(1, ((ReplySlowActor)actor.Runner.Actor!).GetMessages("TestAskMessageToSlowActor"));
    }

    [Fact]
    public async Task TestAskMessageToSlowActorWithTimeout()
    {
        using ActorSystem asx = new();

        IActorRef<ReplySlowActor, string, string> actor = asx.Spawn<ReplySlowActor, string, string>();

        string? reply = await actor.Ask("TestAskMessageToSlowActorWithTimeout", TimeSpan.FromSeconds(5));
        Assert.NotNull(reply);
        Assert.Equal("TestAskMessageToSlowActorWithTimeout", reply);

        Assert.Equal(1, ((ReplySlowActor)actor.Runner.Actor!).GetMessages("TestAskMessageToSlowActorWithTimeout"));
    }

    [Fact]
    public async Task TestAskMessageToSlowActorTriggerTimeout()
    {
        using ActorSystem asx = new();

        IActorRef<ReplySlowActor, string, string> actor = asx.Spawn<ReplySlowActor, string, string>();

        AskTimeoutException exception = await Assert.ThrowsAsync<AskTimeoutException>(async () => await TriggerTimeout(actor));
        Assert.Equal("Timeout after 00:00:01 waiting for a reply", exception.Message);

        await asx.Wait();

        Assert.Equal(1, ((ReplySlowActor)actor.Runner.Actor!).GetMessages("TestAskMessageToSlowActorTriggerTimeout"));
    }

    private async Task TriggerTimeout(IActorRef<ReplySlowActor, string, string> actor)
    {
        await actor.Ask("TestAskMessageToSlowActorTriggerTimeout", TimeSpan.FromSeconds(1));        
    }
}
