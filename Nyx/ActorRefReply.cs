
namespace Nyx;

/// <summary>
/// Represents an actor reference.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ActorRef<TActor, TRequest, TResponse> : IActorRef<TActor, TRequest, TResponse> where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    private readonly ActorRunner<TActor, TRequest, TResponse> runner;

    public ActorRunner<TActor, TRequest, TResponse> Runner => runner;

    public ActorRef(ActorRunner<TActor, TRequest, TResponse> runner)
    {
        this.runner = runner;
    }

    public void Send(TRequest message)
    {
        runner.SendAndTryDeliver(message);
    }

    public async Task<TResponse?> Ask(TRequest message)
    {
        ActorMessageReply<TRequest, TResponse> promise = runner.SendAndTryDeliver(message);

        while (!promise.IsCompleted)
            await Task.Yield();

        return promise.Response;
    }
}