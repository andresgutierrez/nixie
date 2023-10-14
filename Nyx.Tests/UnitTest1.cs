
namespace Nyx.Tests;

public sealed class MyActor : IActor<string, string>
{
    private static int numberMessages;

    public static int GetMessages => numberMessages;

    public Task<string> Receive(string message)
    {
        numberMessages++;

        return Task.FromResult(message);
    }
}

public sealed class MyActor2 : IActor<string, string>
{
    private static int numberMessages;

    public static int GetMessages => numberMessages;

    public Task<string> Receive(string message)
    {
        numberMessages++;

        return Task.FromResult(message);
    }
}

public sealed class MyActor3 : IActor<string>
{
    private static int numberMessages;

    public static int GetMessages => numberMessages;

    public async Task Receive(string message)
    {        
        await Task.Yield();

        numberMessages++;        
    }
}

public sealed class MyActor4 : IActor<string>
{
    private static int numberMessages;

    public static int GetMessages => numberMessages;

    public async Task Receive(string message)
    {
        await Task.Yield();

        numberMessages++;
    }
}

public sealed class UnitTest1
{
    [Fact]
    public async Task TestSendMessageToSingleActor()
    {
        ActorSystem asx = new();

        IActorRef<MyActor, string, string> actor = asx.Create<MyActor, string, string>();

        actor.Send("hello 1");

        await asx.Wait();

        Assert.Equal(1, MyActor.GetMessages);
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActor()
    {
        ActorSystem asx = new();

        IActorRef<MyActor2, string, string> actor = asx.Create<MyActor2, string, string>();

        //as.

        actor.Send("hello 1");
        actor.Send("hello 2");
        actor.Send("hello 3");

        await asx.Wait();

        Assert.Equal(3, MyActor2.GetMessages);
    }

    [Fact]
    public async Task TestSendMessageToSingleActorNoResponse()
    {
        ActorSystem asx = new();

        IActorRef<MyActor3, string> actor = asx.Create<MyActor3, string>();

        actor.Send("hello 1");

        await asx.Wait();

        Assert.Equal(1, MyActor3.GetMessages);
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponse()
    {
        ActorSystem asx = new();

        IActorRef<MyActor4, string> actor = asx.Create<MyActor4, string>();

        //as.

        actor.Send("hello 1");
        actor.Send("hello 2");
        actor.Send("hello 3");

        await asx.Wait();

        Assert.Equal(3, MyActor4.GetMessages);
    }
}