
using Nixie.Routers;
using Nixie.Tests.Actors;
using System.Diagnostics.Metrics;

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
    public async void TestCreateRoundRobinRouterReply()
    {
        using ActorSystem asx = new();

        IActorRef<RoundRobinActor<RouteeReplyActor, RouterMessage, RouterResponse>, RouterMessage, RouterResponse> router =
            asx.Spawn<RoundRobinActor<RouteeReplyActor, RouterMessage, RouterResponse>, RouterMessage, RouterResponse>("my-router", 5);

        await router.Ask(new RouterMessage(RouterMessageType.Route, "aaa"));
        await router.Ask(new RouterMessage(RouterMessageType.Route, "bbb"));
        await router.Ask(new RouterMessage(RouterMessageType.Route, "ccc"));
        await router.Ask(new RouterMessage(RouterMessageType.Route, "ddd"));
        await router.Ask(new RouterMessage(RouterMessageType.Route, "eee"));

        Assert.IsAssignableFrom<RoundRobinActor<RouteeReplyActor, RouterMessage, RouterResponse>>(router.Runner.Actor);

        RoundRobinActor<RouteeReplyActor, RouterMessage, RouterResponse> routerActor = (RoundRobinActor<RouteeReplyActor, RouterMessage, RouterResponse>)router.Runner.Actor!;

        foreach (IActorRef<RouteeReplyActor, RouterMessage, RouterResponse> routee in routerActor.Instances)
        {
            Assert.IsAssignableFrom<RouteeReplyActor>(routee.Runner.Actor);

            RouteeReplyActor routeeActor = (RouteeReplyActor)routee.Runner.Actor!;
            Assert.Equal(1, routeeActor.GetMessages());
        }
    }

    [Fact]
    public async void TestCreateConsistentHashRouter()
    {
        using ActorSystem asx = new();

        IActorRef<ConsistentHashActor<RouteeActor, RouterMessage>, RouterMessage> router =
            asx.Spawn<ConsistentHashActor<RouteeActor, RouterMessage>, RouterMessage>("my-router", 5);

        router.Send(new RouterMessage(RouterMessageType.Route, "aaa"));
        router.Send(new RouterMessage(RouterMessageType.Route, "bbb"));
        router.Send(new RouterMessage(RouterMessageType.Route, "ccc"));
        router.Send(new RouterMessage(RouterMessageType.Route, "ddd"));
        router.Send(new RouterMessage(RouterMessageType.Route, "eee"));

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

    [Fact]
    public async void TestCreateConsistentHashRouterReply()
    {
        using ActorSystem asx = new();

        IActorRef<ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse>, RouterMessage, RouterResponse> router =
            asx.Spawn<ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse>, RouterMessage, RouterResponse>("my-router", 5);

        await router.Ask(new RouterMessage(RouterMessageType.Route, "aaa"));
        await router.Ask(new RouterMessage(RouterMessageType.Route, "bbb"));
        await router.Ask(new RouterMessage(RouterMessageType.Route, "ccc"));
        await router.Ask(new RouterMessage(RouterMessageType.Route, "ddd"));
        await router.Ask(new RouterMessage(RouterMessageType.Route, "eee"));

        await asx.Wait();

        Assert.IsAssignableFrom<ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse>>(router.Runner.Actor);

        ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse> routerActor = (ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse>)router.Runner.Actor!;

        foreach (IActorRef<RouteeReplyActor, RouterMessage, RouterResponse> routee in routerActor.Instances)
        {
            Assert.IsAssignableFrom<RouteeReplyActor>(routee.Runner.Actor);

            RouteeReplyActor routeeActor = (RouteeReplyActor)routee.Runner.Actor!;
            Assert.Equal(1, routeeActor.GetMessages());
        }
    }   

    [Fact]
    public async void TestCreateConsistentHashRouterReplyParallel()
    {
        using ActorSystem asx = new();

        IActorRef<ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse>, RouterMessage, RouterResponse> router =
            asx.Spawn<ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse>, RouterMessage, RouterResponse>("my-router", 5);

        Task[] tasks = new Task[5]
        {
            router.Ask(new RouterMessage(RouterMessageType.Route, "aaa")),
            router.Ask(new RouterMessage(RouterMessageType.Route, "bbb")),
            router.Ask(new RouterMessage(RouterMessageType.Route, "ccc")),
            router.Ask(new RouterMessage(RouterMessageType.Route, "ddd")),
            router.Ask(new RouterMessage(RouterMessageType.Route, "eee")),
        };

        await Task.WhenAll(tasks);

        Assert.IsAssignableFrom<ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse>>(router.Runner.Actor);

        ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse> routerActor = (ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse>)router.Runner.Actor!;

        foreach (IActorRef<RouteeReplyActor, RouterMessage, RouterResponse> routee in routerActor.Instances)
        {
            Assert.IsAssignableFrom<RouteeReplyActor>(routee.Runner.Actor);

            RouteeReplyActor routeeActor = (RouteeReplyActor)routee.Runner.Actor!;
            Assert.Equal(1, routeeActor.GetMessages());
        }
    }
}
