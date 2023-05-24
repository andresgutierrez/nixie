
using System.Numerics;

namespace Nyx.Tests;

public class MyActor : IActor<string, string>
{
    public string Receive(string hello)
    {
        return "hello back";
    }
}

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var asx = new ActorSystem();

        var actor = asx.Create<MyActor,string,string>();

        //as.

        actor.Send("hello");
    }
}