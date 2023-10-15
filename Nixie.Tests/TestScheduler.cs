
using Nixie.Tests.Actors;

namespace Nixie.Tests;

public sealed class TestScheduler
{
    [Fact]
    public async void TestCreatePeriodicTimer()
    {
        using ActorSystem asx = new();

        IActorRef<PeriodicTimerActor, string> actor = asx.Create<PeriodicTimerActor, string>();

        Assert.IsAssignableFrom<PeriodicTimerActor>(actor.Runner.Actor);

        await Task.Delay(5500);

        Assert.True(((PeriodicTimerActor)actor.Runner.Actor!).GetMessages("hello") >= 5);
    }
}