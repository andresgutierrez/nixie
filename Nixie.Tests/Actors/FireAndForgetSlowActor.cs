﻿
namespace Nixie.Tests.Actors;

public sealed class FireAndForgetSlowActor : IActor<string>
{
    private readonly Dictionary<string, int> receivedMessages = new();

    public FireAndForgetSlowActor(IActorContext<FireAndForgetSlowActor, string> context)
    {

    }    

    public int GetMessages(string id)
    {
        return receivedMessages.GetValueOrDefault(id, 0);
    }

    private void IncrMessage(string id)
    {
        if (!receivedMessages.TryGetValue(id, out int value))
            receivedMessages.Add(id, 1);
        else
            receivedMessages[id] = ++value;
    }

    public async Task Receive(string message)
    {
        await Task.Delay(100);

        IncrMessage(message);
    }
}