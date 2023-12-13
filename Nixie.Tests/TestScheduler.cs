
using Microsoft.Extensions.Logging;
using Nixie.Tests.Actors;

namespace Nixie.Tests;

[Collection("Nixie")]
public sealed class TestScheduler
{
    [Fact]
    public async void TestCreatePeriodicTimer()
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Nixie", LogLevel.Debug)
                .AddConsole();
        });

        ILogger logger = loggerFactory.CreateLogger<TestSendMessages>();

        using ActorSystem asx = new(logger: logger);

        IActorRef<PeriodicTimerActor, string> actor = asx.Spawn<PeriodicTimerActor, string>();

        Assert.IsAssignableFrom<PeriodicTimerActor>(actor.Runner.Actor);

        await Task.Delay(5500);

        int numberMessages = ((PeriodicTimerActor)actor.Runner.Actor!).GetMessages("hello");

        Assert.Equal(6, numberMessages);
    }

    [Fact]
    public async void TestCreatePeriodicTimerExternalStop()
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Nixie", LogLevel.Debug)
                .AddConsole();
        });

        ILogger logger = loggerFactory.CreateLogger<TestSendMessages>();

        using ActorSystem asx = new(logger: logger);

        IActorRef<PeriodicTimerActor, string> actor = asx.Spawn<PeriodicTimerActor, string>();

        Assert.IsAssignableFrom<PeriodicTimerActor>(actor.Runner.Actor);

        await Task.Delay(2500);

        asx.StopPeriodicTimer(actor, "periodic-timer");

        int numberMessages = ((PeriodicTimerActor)actor.Runner.Actor!).GetMessages("hello");

        Assert.Equal(3, numberMessages);

        await Task.Delay(2500);

        numberMessages = ((PeriodicTimerActor)actor.Runner.Actor!).GetMessages("hello");

        Assert.Equal(3, numberMessages);
    }

    [Fact]
    public void TestCreatePeriodicTimerExternalStopTwice()
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Nixie", LogLevel.Debug)
                .AddConsole();
        });

        ILogger logger = loggerFactory.CreateLogger<TestSendMessages>();

        using ActorSystem asx = new(logger: logger);

        IActorRef<PeriodicTimerActor, string> actor = asx.Spawn<PeriodicTimerActor, string>();

        Assert.IsAssignableFrom<PeriodicTimerActor>(actor.Runner.Actor);

        NixieException exception = Assert.Throws<NixieException>(() =>
        {
            asx.StopPeriodicTimer(actor, "periodic-timer");
            asx.StopPeriodicTimer(actor, "periodic-timer");
        });

        Assert.Equal("There is no timer with this name.", exception.Message);
    }

    [Fact]
    public async void TestCreateOnceTimer()
    {
        using ActorSystem asx = new();

        IActorRef<OnceTimerActor, OnceTimerMessage> actor = asx.Spawn<OnceTimerActor, OnceTimerMessage>();

        Assert.IsAssignableFrom<OnceTimerActor>(actor.Runner.Actor);

        await Task.Delay(2000);

        int numberMessages = ((OnceTimerActor)actor.Runner.Actor!).GetMessages();

        Assert.Equal(1, numberMessages);
    }

    [Fact]
    public async void TestCreateOnceTimerExternal()
    {
        using ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Spawn<FireAndForgetActor, string>();

        Assert.IsAssignableFrom<FireAndForgetActor>(actor.Runner.Actor);

        asx.ScheduleOnce(actor, "TestCreateOnceTimerExternal", TimeSpan.FromMilliseconds(10));
        asx.ScheduleOnce(actor, "TestCreateOnceTimerExternal", TimeSpan.FromMilliseconds(20));
        asx.ScheduleOnce(actor, "TestCreateOnceTimerExternal", TimeSpan.FromMilliseconds(30));
        asx.ScheduleOnce(actor, "TestCreateOnceTimerExternal", TimeSpan.FromMilliseconds(40));
        asx.ScheduleOnce(actor, "TestCreateOnceTimerExternal", TimeSpan.FromMilliseconds(50));

        await Task.Delay(1000);

        int numberMessages = ((FireAndForgetActor)actor.Runner.Actor!).GetMessages("TestCreateOnceTimerExternal");

        Assert.Equal(5, numberMessages);
    }

    [Fact]
    public async void TestCreateOnceTimerExternalReply()
    {
        using ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Spawn<ReplyActor, string, string>();

        Assert.IsAssignableFrom<ReplyActor>(actor.Runner.Actor);

        asx.ScheduleOnce(actor, "TestCreateOnceTimerExternal", TimeSpan.FromMilliseconds(10));
        asx.ScheduleOnce(actor, "TestCreateOnceTimerExternal", TimeSpan.FromMilliseconds(20));
        asx.ScheduleOnce(actor, "TestCreateOnceTimerExternal", TimeSpan.FromMilliseconds(30));
        asx.ScheduleOnce(actor, "TestCreateOnceTimerExternal", TimeSpan.FromMilliseconds(40));
        asx.ScheduleOnce(actor, "TestCreateOnceTimerExternal", TimeSpan.FromMilliseconds(50));

        await Task.Delay(1000);

        int numberMessages = ((ReplyActor)actor.Runner.Actor!).GetMessages("TestCreateOnceTimerExternal");

        Assert.Equal(5, numberMessages);
    }
}