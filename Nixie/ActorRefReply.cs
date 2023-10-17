
namespace Nixie;

/// <summary>
/// Represents an actor reference.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ActorRef<TActor, TRequest, TResponse> : IGenericActorRef, IActorRef<TActor, TRequest, TResponse>
    where TActor : IActor<TRequest, TResponse> where TRequest : class where TResponse : class
{
    private readonly ActorRunner<TActor, TRequest, TResponse> runner;

    /// <summary>
    /// Returns a reference to the actor's runner
    /// </summary>
    public ActorRunner<TActor, TRequest, TResponse> Runner => runner;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="runner"></param>
    public ActorRef(ActorRunner<TActor, TRequest, TResponse> runner)
    {
        this.runner = runner;
    }

    /// <summary>
    /// Passes a message to the actor without expecting a response and without specifying a sender
    /// </summary>
    /// <param name="message"></param>
    public void Send(TRequest message)
    {
        runner.SendAndTryDeliver(message, null);
    }

    /// <summary>
    /// Passes a message to the actor without expecting a response and specifying a sender
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>    
    public void Send(TRequest message, IGenericActorRef sender)
    {
        runner.SendAndTryDeliver(message, sender);
    }

    /// <summary>
    /// Sends a message to actor expecting a response and without specifying a sender
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<TResponse?> Ask(TRequest message)
    {
        ActorMessageReply<TRequest, TResponse> promise = runner.SendAndTryDeliver(message, null);

        while (!promise.IsCompleted)
            await Task.Yield();

        return promise.Response;
    }

    /// <summary>
    /// Sends a message to the actor and expects a response
    /// An exception will be thrown if the timeout limit is reached
    /// </summary>
    /// <param name="message"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<TResponse?> Ask(TRequest message, TimeSpan timeout)
    {
        ActorMessageReply<TRequest, TResponse> promise = runner.SendAndTryDeliver(message, null);

        while (!promise.IsCompleted)
            await Task.Yield();

        return promise.Response;
    }

    /// <summary>
    /// Sends a message to actor expecting a response and specifying the sender
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public async Task<TResponse?> Ask(TRequest message, IGenericActorRef sender)
    {
        ActorMessageReply<TRequest, TResponse> promise = runner.SendAndTryDeliver(message, sender);

        while (!promise.IsCompleted)
            await Task.Yield();

        return promise.Response;
    }

    /// <summary>
    /// Sends a message to actor expecting a response and specifying the sender
    /// An exception will be thrown if the timeout limit is reached
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<TResponse?> Ask(TRequest message, IGenericActorRef sender, TimeSpan timeout)
    {
        ActorMessageReply<TRequest, TResponse> promise = runner.SendAndTryDeliver(message, sender);

        while (!promise.IsCompleted)
            await Task.Yield();

        return promise.Response;
    }
}