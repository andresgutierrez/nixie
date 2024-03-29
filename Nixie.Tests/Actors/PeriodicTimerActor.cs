﻿
namespace Nixie.Tests.Actors;

public sealed class PeriodicTimerActor : IActor<string>
{
    private readonly Dictionary<string, int> receivedMessages = new();

    public PeriodicTimerActor(IActorContext<PeriodicTimerActor, string> context)
    {
        context.ActorSystem.StartPeriodicTimer(context.Self, "periodic-timer", "hello", TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    public int GetMessages(string id)
    {
        if (receivedMessages.TryGetValue(id, out int number))
            return number;

        return 0;
    }

    public void IncrMessage(string id)
    {
        if (!receivedMessages.ContainsKey(id))
            receivedMessages.Add(id, 1);
        else
            receivedMessages[id]++;
    }

    public async Task Receive(string message)
    {
        await Task.Yield();

        //Console.WriteLine("hello");

        IncrMessage(message);
    }
}