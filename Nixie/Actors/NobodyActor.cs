﻿
namespace Nixie.Actors;

/// <summary>
/// Represents a nobody actor.
/// </summary>
public sealed class NobodyActor : IActor<object>
{
    public NobodyActor(IActorContext<NobodyActor, object> _)
    {

    }

    public Task Receive(object message)
    {
        return Task.CompletedTask; // Discard all messages
    }
}
