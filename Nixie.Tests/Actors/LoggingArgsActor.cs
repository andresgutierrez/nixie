
using Microsoft.Extensions.Logging;

namespace Nixie.Tests.Actors;

public class LoggingArgsActor : IActor<string>
{    
    private readonly ILogger<TestLogging> logger;

    public LoggingArgsActor(IActorContext<LoggingArgsActor, string> _, ILogger<TestLogging> logger)
    {
        this.logger = logger;
    }

    public async Task Receive(string message)
    {
        await Task.Yield();

        logger.LogInformation("Message: {Message}", message);
    }
}

