
namespace Nyx;

public sealed class ActorRef<TActor, TRequest, TResponse> : IActorRef<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    private readonly ActorContext<TActor, TRequest, TResponse> context;

    public ActorRef(ActorContext<TActor, TRequest, TResponse> context)
    {
        this.context = context;
    }

    public void Send(TRequest message)
    {
        context.Inbox.Enqueue(message);
    }
}
