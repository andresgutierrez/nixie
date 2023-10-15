
namespace Nyx.Tests;

public sealed class ReplyActor : IActor<string, string>
{
    private readonly Dictionary<string, int> receivedMessages = new();

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

    public Task<string> Receive(string message)
    {
        IncrMessage(message);

        return Task.FromResult(message);
    }
}

public sealed class FireAndForgetActor : IActor<string>
{
    private readonly Dictionary<string, int> receivedMessages = new();

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

public sealed class FireAndForgetSlowActor : IActor<string>
{
    private readonly Dictionary<string, int> receivedMessages = new();

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
        await Task.Delay(1000);

        IncrMessage(message);
    }
}

public sealed class UnitTest1
{
    [Fact]
    public async Task TestSendMessageToSingleActor()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Create<ReplyActor, string, string>();

        actor.Send("TestSendMessageToSingleActor");

        await Task.Delay(2000);

        Assert.Equal(1, ((ReplyActor)actor.Context.Actor).GetMessages("TestSendMessageToSingleActor"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActor()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Create<ReplyActor, string, string>();

        actor.Send("TestSendMultipleMessageToSingleActor");
        actor.Send("TestSendMultipleMessageToSingleActor");
        actor.Send("TestSendMultipleMessageToSingleActor");

        await Task.Delay(5000);

        Assert.Equal(3, ((ReplyActor)actor.Context.Actor).GetMessages("TestSendMultipleMessageToSingleActor"));
    }

    [Fact]
    public async Task TestAskMessageToSingleActor()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Create<ReplyActor, string, string>();

        string? reply = await actor.Ask("TestSendMessageToSingleActor");
        Assert.NotNull(reply);
        Assert.Equal("TestSendMessageToSingleActor", reply);        

        Assert.Equal(1, ((ReplyActor)actor.Context.Actor).GetMessages("TestSendMessageToSingleActor"));
    }

    [Fact]
    public async Task TestSendMessageToSingleActorNoResponse()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Create<FireAndForgetActor, string>();

        actor.Send("TestSendMessageToSingleActorNoResponse");

        await Task.Delay(2000);

        Assert.Equal(1, ((FireAndForgetActor)actor.Context.Actor).GetMessages("TestSendMessageToSingleActorNoResponse"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponse()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Create<FireAndForgetActor, string>("TestSendMultipleMessageToSingleActorNoResponse");

        actor.Send("TestSendMultipleMessageToSingleActorNoResponse");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponse");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponse");

        //await asx.Wait();

        await Task.Delay(2000);

        Assert.Equal(3, ((FireAndForgetActor)actor.Context.Actor).GetMessages("TestSendMultipleMessageToSingleActorNoResponse"));        
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponse2()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Create<FireAndForgetActor, string>("TestSendMultipleMessageToSingleActorNoResponse");

        for (int i = 0; i < 100; i++)
            actor.Send("TestSendMultipleMessageToSingleActorNoResponse");        

        await Task.Delay(2000);

        Assert.Equal(100, ((FireAndForgetActor)actor.Context.Actor).GetMessages("TestSendMultipleMessageToSingleActorNoResponse"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponseSlow()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetSlowActor, string> actor = asx.Create<FireAndForgetSlowActor, string>("TestSendMultipleMessageToSingleActorNoResponseSlow");

        actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");
        actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");

        //await asx.Wait();

        await Task.Delay(5000);

        Assert.Equal(3, ((FireAndForgetSlowActor)actor.Context.Actor).GetMessages("TestSendMultipleMessageToSingleActorNoResponseSlow"));
    }

    [Fact]
    public async Task TestSendMultipleMessageToSingleActorNoResponseSlow2()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetSlowActor, string> actor = asx.Create<FireAndForgetSlowActor, string>("TestSendMultipleMessageToSingleActorNoResponseSlow");

        for (int i = 0; i < 10; i++)
            actor.Send("TestSendMultipleMessageToSingleActorNoResponseSlow");        

        //await asx.Wait();

        await Task.Delay(12000);

        Assert.Equal(10, ((FireAndForgetSlowActor)actor.Context.Actor).GetMessages("TestSendMultipleMessageToSingleActorNoResponseSlow"));
    }
}