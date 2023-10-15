
namespace Nixie;

public sealed class ActorRef<TActor, TRequest> : IActorRef<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    private readonly ActorRunner<TActor, TRequest> runner;

    public ActorRunner<TActor, TRequest> Runner => runner;

    public ActorRef(ActorRunner<TActor, TRequest> runner)
    {
        this.runner = runner;
    }

    public void Send(TRequest message)
    {
        runner.SendAndTryDeliver(message);
    }
}
