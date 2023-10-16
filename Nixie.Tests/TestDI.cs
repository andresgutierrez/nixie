
using Microsoft.Extensions.DependencyInjection;
using Nixie.Tests.Actors;

namespace Nixie.Tests;

internal interface ISomeService
{
    public int GetValue();
}

internal sealed class SomeService : ISomeService
{
    public int GetValue()
    {
        return 5;
    }
}

public class TestDI
{
    [Fact]
    public async Task TestBuildActorWithDependencyInjection()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<ISomeService, SomeService>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        using ActorSystem asx = new(serviceProvider);

        IActorRef<DiAwareActor, string> actor = asx.Spawn<DiAwareActor, string>();

        Assert.IsAssignableFrom<DiAwareActor>(actor.Runner.Actor);

        actor.Send("message");

        await asx.Wait();

        Assert.Equal(5, ((DiAwareActor)actor.Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestBuildActorWithDependencyInjectionAndSendMany()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<ISomeService, SomeService>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        using ActorSystem asx = new(serviceProvider);

        IActorRef<DiAwareActor, string> actor = asx.Spawn<DiAwareActor, string>();

        Assert.IsAssignableFrom<DiAwareActor>(actor.Runner.Actor);

        for (int i = 0; i < 10; i++)
            actor.Send("message");

        await asx.Wait();

        Assert.Equal(50, ((DiAwareActor)actor.Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestBuildActorWithDependencyInjectionAndArgs()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<ISomeService, SomeService>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        using ActorSystem asx = new(serviceProvider);

        IActorRef<DiAwareArgsActor, string> actor = asx.Spawn<DiAwareArgsActor, string>(null, 100);

        Assert.IsAssignableFrom<DiAwareArgsActor>(actor.Runner.Actor);

        actor.Send("message");

        await asx.Wait();

        Assert.Equal(105, ((DiAwareArgsActor)actor.Runner.Actor!).GetMessages());
    }

    [Fact]
    public async Task TestBuildActorWithDependencyInjectionAndArgsAndSendMany()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<ISomeService, SomeService>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        using ActorSystem asx = new(serviceProvider);

        IActorRef<DiAwareArgsActor, string> actor = asx.Spawn<DiAwareArgsActor, string>(null, 100);

        Assert.IsAssignableFrom<DiAwareArgsActor>(actor.Runner.Actor);

        for (int i = 0; i < 10; i++)
            actor.Send("message");

        await asx.Wait();

        Assert.Equal(1050, ((DiAwareArgsActor)actor.Runner.Actor!).GetMessages());
    }
}

