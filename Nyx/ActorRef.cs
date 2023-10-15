
namespace Nyx;

public sealed class ActorRef<TActor, TRequest> : IActorRef<TActor, TRequest> where TActor : IActor<TRequest> where TRequest : class
{
    private readonly ActorContext<TActor, TRequest> context;

    public ActorContext<TActor, TRequest> Context => context;

    public ActorRef(ActorContext<TActor, TRequest> context)
    {
        this.context = context;
    }

    public void Send(TRequest message)
    {
        context.SendAndTryDeliver(message);
    }
}
