﻿
namespace Nixie.Tests.Actors;

public sealed class ShutdownActor : IActor<string>
{
    private int receivedMessages;

    public ShutdownActor(IActorContext<ShutdownActor, string> _)
    {
        
    }

    public int GetMessages()
    {
        return receivedMessages;
    }

    private void IncrMessage()
    {
        receivedMessages++;
    }

    public async Task Receive(string message)
    {
        await Task.Yield();

        IncrMessage();
    }
}
