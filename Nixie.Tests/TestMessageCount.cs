
using Nixie.Tests.Actors;

namespace Nixie.Tests;

[Collection("Nixie")]
public sealed class TestMessageCount
{
    [Fact]
    public async Task TestMessageCountEmpty()
    {
        using ActorSystem asx = new();

        IActorRef<MessageCountActor, MessageCountRequest, MessageCountResponse> actor = asx.Spawn<MessageCountActor, MessageCountRequest, MessageCountResponse>();        

        MessageCountResponse? message = await actor.Ask(new MessageCountRequest());
        Assert.NotNull(message);

        Assert.True(message.Counter.HasValue);
        Assert.Equal(0, message.Counter.Value);
    }
}