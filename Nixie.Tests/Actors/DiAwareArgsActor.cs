
namespace Nixie.Tests.Actors;

internal sealed class DiAwareArgsActor : IActor<string>
{
    private int receivedMessages;

    private readonly ISomeService someService;

    private readonly int extra;

    public DiAwareArgsActor(IActorContext<DiAwareArgsActor, string> _, ISomeService someService, int extra)
    {
        this.someService = someService;
        this.extra = extra;        
    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    public void IncrMessage(int number)
    {
        receivedMessages += number;
    }

    public async Task Receive(string message)
    {
        await Task.Yield();

        IncrMessage(someService.GetValue() + extra);
    }
}

