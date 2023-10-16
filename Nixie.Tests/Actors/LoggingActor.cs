
using Microsoft.Extensions.Logging;

namespace Nixie.Tests.Actors;

public class LoggingActor : IActor<string>
{
    private readonly IActorContext<LoggingActor, string> context;

    public LoggingActor(IActorContext<LoggingActor, string> context)
	{
        this.context = context;
	}

    public async Task Receive(string message)
    {
        await Task.Yield();

        context.Logger?.LogInformation("Message: {Message}", message);
    }
}

