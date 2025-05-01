
using Nixie.Routers;
using Nixie.Tests.Actors;

namespace Nixie.Tests;

[Collection("Nixie")]
public sealed class TestRouters
{
    [Fact]
    public async Task TestCreateRoundRobinRouter()
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
    public async Task TestCreateRoundRobinRouterStruct()
    {
        using ActorSystem asx = new();

        IActorRefStruct<RoundRobinActorStruct<RouteeActorStruct, RouterMessageStruct>, RouterMessageStruct> router =
            asx.SpawnStruct<RoundRobinActorStruct<RouteeActorStruct, RouterMessageStruct>, RouterMessageStruct>("my-router", 5);

        router.Send(new RouterMessageStruct(RouterMessageType.Route, "aaa"));
        router.Send(new RouterMessageStruct(RouterMessageType.Route, "bbb"));
        router.Send(new RouterMessageStruct(RouterMessageType.Route, "ccc"));
        router.Send(new RouterMessageStruct(RouterMessageType.Route, "ddd"));
        router.Send(new RouterMessageStruct(RouterMessageType.Route, "eee"));

        await asx.Wait();

        Assert.IsAssignableFrom<RoundRobinActorStruct<RouteeActorStruct, RouterMessageStruct>>(router.Runner.Actor);

        RoundRobinActorStruct<RouteeActorStruct, RouterMessageStruct> routerActor = (RoundRobinActorStruct<RouteeActorStruct, RouterMessageStruct>)router.Runner.Actor!;

        foreach (IActorRefStruct<RouteeActorStruct, RouterMessageStruct> routee in routerActor.Instances)
        {
            Assert.IsAssignableFrom<RouteeActorStruct>(routee.Runner.Actor);

            RouteeActorStruct routeeActor = (RouteeActorStruct)routee.Runner.Actor!;
            Assert.Equal(1, routeeActor.GetMessages());
        }
    }

    [Fact]
    public async Task TestCreateRoundRobinRouterExt()
    {
        using ActorSystem asx = new();

        IActorRef<RoundRobinActor<RouteeActor, RouterMessage>, RouterMessage> router = asx.CreateRoundRobinRouter<RouteeActor, RouterMessage>("my-router", 5);

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
    public async Task TestCreateRoundRobinRouterExtInstances()
    {
        using ActorSystem asx = new();

        List<IActorRef<RouteeActor, RouterMessage>> instances = new();

        for (int i = 0; i < 5; i++)
            instances.Add(asx.Spawn<RouteeActor, RouterMessage>());

        IActorRef<RoundRobinActor<RouteeActor, RouterMessage>, RouterMessage> router = asx.CreateRoundRobinRouter("my-router", instances);

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
    public async Task TestCreateRoundRobinRouterSlowSend()
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
    public async Task TestCreateRoundRobinRouterReply()
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
    public async Task TestCreateRoundRobinRouterReplyExtInstances()
    {
        using ActorSystem asx = new();

        List<IActorRef<RouteeReplyActor, RouterMessage, RouterResponse>> instances = new();

        for (int i = 0; i < 5; i++)
            instances.Add(asx.Spawn<RouteeReplyActor, RouterMessage, RouterResponse>());

        IActorRef<RoundRobinActor<RouteeReplyActor, RouterMessage, RouterResponse>, RouterMessage, RouterResponse> router =
            asx.CreateRoundRobinRouter("my-router", instances);

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
    public async Task TestCreateRoundRobinRouterReplyExt()
    {
        using ActorSystem asx = new();

        IActorRef<RoundRobinActor<RouteeReplyActor, RouterMessage, RouterResponse>, RouterMessage, RouterResponse> router =
            asx.CreateRoundRobinRouter<RouteeReplyActor, RouterMessage, RouterResponse>("my-router", 5);

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
    public async Task TestCreateConsistentHashRouter()
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
    public async Task TestCreateConsistentHashRouterExt()
    {
        using ActorSystem asx = new();

        List<IActorRef<RouteeActor, RouterMessage>> instances = new();

        for (int i = 0; i < 5; i++)
            instances.Add(asx.Spawn<RouteeActor, RouterMessage>());

        IActorRef<ConsistentHashActor<RouteeActor, RouterMessage>, RouterMessage> router =
            asx.Spawn<ConsistentHashActor<RouteeActor, RouterMessage>, RouterMessage>("my-router", instances);

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
    public async Task TestCreateConsistentHashRouterReply()
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
    public async Task TestCreateConsistentHashRouterReplyParallel()
    {
        using ActorSystem asx = new();

        IActorRef<ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse>, RouterMessage, RouterResponse> router =
            asx.Spawn<ConsistentHashActor<RouteeReplyActor, RouterMessage, RouterResponse>, RouterMessage, RouterResponse>("my-router", 5);

        Task[] tasks = {
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
    
    [Fact]
    public async Task TestCreateBalancingRouterExt()
    {
        using ActorSystem asx = new();

        IActorRef<BalancingActor<RouteeActor, RouterMessage>, RouterMessage> router =
            asx.Spawn<BalancingActor<RouteeActor, RouterMessage>, RouterMessage>("my-router", 5);
        
        router.Send(new RouterMessage(RouterMessageType.Route, "aaa"));
        router.Send(new RouterMessage(RouterMessageType.Route, "bbb"));
        router.Send(new RouterMessage(RouterMessageType.Route, "ccc"));
        router.Send(new RouterMessage(RouterMessageType.Route, "ddd"));
        router.Send(new RouterMessage(RouterMessageType.Route, "eee"));

        await asx.Wait();

        Assert.IsAssignableFrom<BalancingActor<RouteeActor, RouterMessage>>(router.Runner.Actor);
    }
    
    [Fact]
    public async Task TestCreateBalancingSlowRouterExt()
    {
        using ActorSystem asx = new();

        IActorRef<BalancingActor<RouteeSlowActor, RouterMessage>, RouterMessage> router =
            asx.Spawn<BalancingActor<RouteeSlowActor, RouterMessage>, RouterMessage>("my-router", 5);
        
        router.Send(new RouterMessage(RouterMessageType.Route, "aaa"));
        router.Send(new RouterMessage(RouterMessageType.Route, "bbb"));
        router.Send(new RouterMessage(RouterMessageType.Route, "ccc"));
        router.Send(new RouterMessage(RouterMessageType.Route, "ddd"));
        router.Send(new RouterMessage(RouterMessageType.Route, "eee"));

        await asx.Wait();

        Assert.IsAssignableFrom<BalancingActor<RouteeSlowActor, RouterMessage>>(router.Runner.Actor);
    }
}
