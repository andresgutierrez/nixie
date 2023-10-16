
using Nixie.Tests.Actors;
using Microsoft.Extensions.Logging;

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

        actor.Send("message");

        await asx.Wait();

        //Assert.Equal(5, ((LoggingActor)actor.Runner.Actor!).GetMessages());
    }
}

