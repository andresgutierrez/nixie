
namespace Nixie.Tests;

public class SomeResultAsync
{

}

[Collection("Nixie")]
public class TestLazyAsync
{
    [Fact]
    public async Task TestAskMessageToSingleActor()
    {
        LazyTask<SomeResultAsync> task = CreateResultAsync();
        Assert.NotNull(await task);
    }

    private async LazyTask<SomeResultAsync> CreateResultAsync()
    {
        await Task.Delay(100);
        return new SomeResultAsync();
    }
}