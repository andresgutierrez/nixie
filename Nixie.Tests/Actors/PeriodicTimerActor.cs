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
        await Task.CompletedTask;

        //Console.WriteLine("hello");

        IncrMessage(message);
    }
}