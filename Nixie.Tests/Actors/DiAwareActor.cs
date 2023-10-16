
namespace Nixie.Tests.Actors;

internal sealed class DiAwareActor : IActor<string>
{
    private int receivedMessages;

    private readonly ISomeService someService;

    public DiAwareActor(IActorContext<DiAwareActor, string> _, ISomeService someService)
    {
        this.someService = someService;
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

        IncrMessage(someService.GetValue());
    }
}

