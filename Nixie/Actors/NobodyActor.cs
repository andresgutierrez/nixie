
namespace Nixie.Actors;

/// <summary>
/// Represents a nobody actor.
/// </summary>
public sealed class NobodyActor : IActor<object>
{
    public NobodyActor(IActorContext<NobodyActor, object> _)
    {

    }

    public async Task Receive(object message)
    {
        await Task.Yield(); // Discard all messages
    }
}
