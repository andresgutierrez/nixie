
using System.Numerics;

namespace Nyx.Tests;

public sealed class MyActor : IActor<string, string>
{
    public Task<string> Receive(string message)
    {
        Console.WriteLine(message);

        return Task.FromResult(message);
    }
}

public sealed class MyActor2 : IActor<string>
{
    public async Task Receive(string message)
    {
        await Task.Yield();

        Console.WriteLine(message);        
    }
}

public sealed class UnitTest1
{
    [Fact]
    public async Task TestSendMessageToSingleActor()
    {
        ActorSystem asx = new();

        var actor = asx.Create<MyActor,string,string>();        

        actor.Send("hello 1");        

        await asx.Run();
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActor()
    {
        ActorSystem asx = new();

        var actor = asx.Create<MyActor, string, string>();

        //as.

        actor.Send("hello 1");
        actor.Send("hello 2");
        actor.Send("hello 3");

        await asx.Run();
    }

    [Fact]
    public async Task TestSendMessageToSingleActorNoResponse()
    {
        ActorSystem asx = new();

        var actor = asx.Create<MyActor2, string>();

        actor.Send("hello 1");

        await asx.Run();
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponse()
    {
        ActorSystem asx = new();

        var actor = asx.Create<MyActor2, string>();

        //as.

        actor.Send("hello 1");
        actor.Send("hello 2");
        actor.Send("hello 3");

        await asx.Run();
    }
}