
using Nyx.Tests.Actors;

namespace Nyx.Tests;

public sealed class TestBasics
{
    [Fact]
    public void TestCreateFireAndForgetActor()
    {
        ActorSystem asx = new();

        IActorRef<FireAndForgetActor, string> actor = asx.Create<FireAndForgetActor, string>();
      
        Assert.IsAssignableFrom<FireAndForgetActor>(actor.Runner.Actor);
    }

    [Fact]
    public void TestCreateReplyActor()
    {
        ActorSystem asx = new();

        IActorRef<ReplyActor, string, string> actor = asx.Create<ReplyActor, string, string>();

        Assert.IsAssignableFrom<ReplyActor>(actor.Runner.Actor);
    }
}