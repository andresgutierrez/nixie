﻿
namespace Nixie.Tests.Actors;

public sealed class ReplyActor : IActor<string, string>
{
    private readonly Dictionary<string, int> receivedMessages = new();

    public ReplyActor(IActorContext<ReplyActor, string, string> _)
    {

    }

    public int GetMessages(string id)
    {
        if (receivedMessages.TryGetValue(id, out int number))
            return number;

        return 0;
    }

    public void IncrMessage(string id)
    {
        if (!receivedMessages.TryGetValue(id, out int value))
            receivedMessages.Add(id, 1);
        else
            receivedMessages[id] = ++value;
    }

    public Task<string?> Receive(string message)
    {
        IncrMessage(message);

        return Task.FromResult<string?>(message);
    }
}