
namespace Nyx;

public sealed class ActorRef<TActor, TRequest, TResponse> : IActorRef<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    private readonly ActorContext<TActor, TRequest, TResponse> context;

    public ActorContext<TActor, TRequest, TResponse> Context => context;

    public ActorRef(ActorContext<TActor, TRequest, TResponse> context)
    {
        this.context = context;
    }

    public void Send(TRequest message)
    {
        context.SendAndTryDeliver(message);
    }

    public async Task<TResponse?> Ask(TRequest message)
    {
        ActorMessageReply<TRequest, TResponse> promise = context.SendAndTryDeliver(message);

        while (!promise.IsCompleted)
            await Task.Yield();

        return promise.Response;
    }
}