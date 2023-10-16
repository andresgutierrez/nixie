
using Nixie.Routers;
using Nixie.Tests.Actors;

namespace Nixie.Tests;

[Collection("Nixie")]
public sealed class TestRouters
{
    [Fact]
    public async void TestCreateRoundRobinRouter()
    {
        using ActorSystem asx = new();

        IActorRef<RoundRobinActor<RouteeActor, RouterMessage>, RouterMessage> router =
            asx.Spawn<RoundRobinActor<RouteeActor, RouterMessage>, RouterMessage>("my-router", 5);

        router.Send(new RouterMessage(RouterMessageType.Route, "aaa"));
        router.Send(new RouterMessage(RouterMessageType.Route, "bbb"));
        router.Send(new RouterMessage(RouterMessageType.Route, "ccc"));
        router.Send(new RouterMessage(RouterMessageType.Route, "ddd"));
        router.Send(new RouterMessage(RouterMessageType.Route, "eee"));

        await asx.Wait();

        Assert.IsAssignableFrom<RoundRobinActor<RouteeActor, RouterMessage>>(router.Runner.Actor);

        RoundRobinActor<RouteeActor, RouterMessage> routerActor = (RoundRobinActor<RouteeActor, RouterMessage>)router.Runner.Actor!;

        foreach (IActorRef<RouteeActor, RouterMessage> routee in routerActor.Instances)
        {
            Assert.IsAssignableFrom<RouteeActor>(routee.Runner.Actor);

            RouteeActor routeeActor = (RouteeActor)routee.Runner.Actor!;
            Assert.Equal(1, routeeActor.GetMessages());
        }
    }

    [Fact]
    public async void TestCreateRoundRobinRouterSlowSend()
    {
        using ActorSystem asx = new();

        IActorRef<RoundRobinActor<RouteeSlowActor, RouterMessage>, RouterMessage> router =
            asx.Spawn<RoundRobinActor<RouteeSlowActor, RouterMessage>, RouterMessage>("my-router", 5);

        for (int i = 0; i < 25; i++)
            router.Send(new RouterMessage(RouterMessageType.Route, "aaa"));

        await asx.Wait();

        Assert.IsAssignableFrom<RoundRobinActor<RouteeSlowActor, RouterMessage>>(router.Runner.Actor);

        RoundRobinActor<RouteeSlowActor, RouterMessage> routerActor = (RoundRobinActor<RouteeSlowActor, RouterMessage>)router.Runner.Actor!;

        foreach (IActorRef<RouteeSlowActor, RouterMessage> routee in routerActor.Instances)
        {
            Assert.IsAssignableFrom<RouteeSlowActor>(routee.Runner.Actor);

            RouteeSlowActor routeeActor = (RouteeSlowActor)routee.Runner.Actor!;
            Assert.Equal(5, routeeActor.GetMessages());
        }
    }

    [Fact]
    public async void TestCreateConsistentHashRouter()
    {
        using ActorSystem asx = new();

        IActorRef<ConsistentHashActor<RouteeActor, RouterMessage>, RouterMessage> router =
            asx.Spawn<ConsistentHashActor<RouteeActor, RouterMessage>, RouterMessage>("my-router", 5);

        router.Send(new RouterMessage(RouterMessageType.Route, "cf3be82f-7509-4450-9fc6-747490b4696d")); // 2
        router.Send(new RouterMessage(RouterMessageType.Route, "f9d4c32c-57ea-4826-a632-2bbaf349679e")); // 4
        router.Send(new RouterMessage(RouterMessageType.Route, "11e27c2a-f8a1-4d8c-a350-7c81b5deb29e")); // 1
        router.Send(new RouterMessage(RouterMessageType.Route, "35000df2-df97-4a90-96f9-78887965c9e8")); // 0
        router.Send(new RouterMessage(RouterMessageType.Route, "eac37733-0ec9-440b-98af-553fb65b189f")); // 3

        await asx.Wait();

        Assert.IsAssignableFrom<ConsistentHashActor<RouteeActor, RouterMessage>>(router.Runner.Actor);

        ConsistentHashActor<RouteeActor, RouterMessage> routerActor = (ConsistentHashActor<RouteeActor, RouterMessage>)router.Runner.Actor!;

        foreach (IActorRef<RouteeActor, RouterMessage> routee in routerActor.Instances)
        {
            Assert.IsAssignableFrom<RouteeActor>(routee.Runner.Actor);

            RouteeActor routeeActor = (RouteeActor)routee.Runner.Actor!;
            Assert.Equal(1, routeeActor.GetMessages());
        }
    }
}
