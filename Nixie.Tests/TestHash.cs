
using Nixie.Utils;

namespace Nixie.Tests;

[Collection("Nixie")]
public class TestHash
{
    [Fact]
    public void TestHashFunc()
    {
        Assert.Equal(Hash.Get("hello"), Hash.Get("hello"));
        Assert.Equal(Hash.Get(-100), Hash.Get(100));
    }
}