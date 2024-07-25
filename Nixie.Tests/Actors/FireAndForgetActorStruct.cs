﻿
namespace Nixie.Tests.Actors;

public sealed class FireAndForgetActorStruct : IActorStruct<int>
{
    private readonly Dictionary<int, int> receivedMessages = new();

    public FireAndForgetActorStruct(IActorContextStruct<FireAndForgetActorStruct, int> _)
    {

    }

    public int GetMessages(int id)
    {
        if (receivedMessages.TryGetValue(id, out int number))
            return number;

        return 0;
    }

    public void IncrMessage(int id)
    {
        if (!receivedMessages.TryGetValue(id, out int value))
            receivedMessages.Add(id, 1);
        else
            receivedMessages[id] = ++value;
    }

    public async Task Receive(int message)
    {
        await Task.CompletedTask;

        //Console.WriteLine("hello");

        IncrMessage(message);
    }
}
