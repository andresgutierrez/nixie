﻿
using Nixie.Tests.Actors;

namespace Nixie.Tests;

[Collection("Nixie")]
public sealed class TestScheduler
{
    [Fact]
    public async void TestCreatePeriodicTimer()
    {
        using ActorSystem asx = new();

        IActorRef<PeriodicTimerActor, string> actor = asx.Spawn<PeriodicTimerActor, string>();

        Assert.IsAssignableFrom<PeriodicTimerActor>(actor.Runner.Actor);

        await Task.Delay(5500);

        int numberMessages = ((PeriodicTimerActor)actor.Runner.Actor!).GetMessages("hello");

        Assert.Equal(6, numberMessages);
    }

    [Fact]
    public async void TestCreatePeriodicTimerExternalStop()
    {
        using ActorSystem asx = new();

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
        using ActorSystem asx = new();

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
}