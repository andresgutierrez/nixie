
using Nixie.Tests.Actors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Nixie.Tests;

public class TestLogging
{
    private readonly ILoggerFactory loggerFactory;

    private readonly ILogger logger;

    public TestLogging()
	{
        loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Nixie", LogLevel.Debug)
                .AddConsole();
        });

        logger = loggerFactory.CreateLogger<TestLogging>();
    }

    [Fact]
    public async Task TestAddLogging()
    {        
        using ActorSystem asx = new(logger: logger);

        IActorRef<LoggingActor, string> actor = asx.Spawn<LoggingActor, string>();

        Assert.IsAssignableFrom<LoggingActor>(actor.Runner.Actor);

        actor.Send("TestAddLogging");

        await asx.Wait();
    }

    [Fact]
    public async Task TestAddLoggingArgs()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton(typeof(ILogger<TestLogging>), logger);

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        using ActorSystem asx = new(serviceProvider);

        IActorRef<LoggingArgsActor, string> actor = asx.Spawn<LoggingArgsActor, string>();

        Assert.IsAssignableFrom<LoggingArgsActor>(actor.Runner.Actor);

        actor.Send("TestAddLoggingArgs");

        await asx.Wait();        
    }
}

