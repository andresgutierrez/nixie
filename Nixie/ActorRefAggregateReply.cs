
namespace Nixie;

/// <summary>
/// Represents an actor reference.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ActorRefAggregate<TActor, TRequest, TResponse> : IGenericActorRef, IActorRefAggregate<TActor, TRequest, TResponse>
    where TActor : IActorAggregate<TRequest, TResponse> where TRequest : class where TResponse : class?
{
    /// <summary>
    /// Returns a reference to the actor's runner
    /// </summary>
    public ActorRunnerAggregate<TActor, TRequest, TResponse> Runner { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="runner"></param>
    public ActorRefAggregate(ActorRunnerAggregate<TActor, TRequest, TResponse> runner)
    {
        Runner = runner;
    }

    /// <summary>
    /// Passes a message to the actor without expecting a response and without specifying a sender
    /// </summary>
    /// <param name="message"></param>
    public void Send(TRequest message)
    {
        Runner.SendAndTryDeliver(message, null, null);
    }

    /// <summary>
    /// Passes a message to the actor without expecting a response and specifying a sender
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    public void Send(TRequest message, IGenericActorRef sender)
    {
        Runner.SendAndTryDeliver(message, sender, null);
    }

    /// <summary>
    /// Passes a message to the actor without expecting a response and specifying a parent promise
    /// </summary>
    /// <param name="message"></param>
    /// <param name="parentPromise"></param>
    public void Send(TRequest message, ActorMessageReply<TRequest, TResponse>? parentPromise)
    {
        Runner.SendAndTryDeliver(message, null, parentPromise);
    }

    /// <summary>
    /// Sends a message to actor expecting a response and without specifying a sender
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>    
    public async Task<TResponse?> Ask(TRequest message)
    {
        TaskCompletionSource<TResponse?> promise = Runner.SendAndTryDeliver(message, null, null);

        return await promise.Task;
    }

    /// <summary>
    /// Sends a message to the actor and expects a response
    /// An exception will be thrown if the timeout limit is reached
    /// </summary>
    /// <param name="message"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="AskTimeoutException"></exception>
    public async Task<TResponse?> Ask(TRequest message, TimeSpan timeout)
    {
        using CancellationTokenSource timeoutCancellationTokenSource = new();

        TaskCompletionSource<TResponse?> completionSource = Runner.SendAndTryDeliver(message, null, null);

        Task<TResponse?> task = completionSource.Task;

        Task completedTask = await Task.WhenAny(
            task,
            Task.Delay(timeout, timeoutCancellationTokenSource.Token)
        );

        if (completedTask == task)
        {
            await timeoutCancellationTokenSource.CancelAsync();
            return await task;
        }

        throw new AskTimeoutException($"Timeout after {timeout} waiting for a reply");
    }

    /// <summary>
    /// Sends a message to actor expecting a response and specifying the sender
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public async Task<TResponse?> Ask(TRequest message, IGenericActorRef sender)
    {
        TaskCompletionSource<TResponse?> promise = Runner.SendAndTryDeliver(message, sender, null);

        return await promise.Task;
    }

    /// <summary>
    /// Sends a message to actor expecting a response and specifying the sender
    /// An exception will be thrown if the timeout limit is reached
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="AskTimeoutException"></exception>
    public async Task<TResponse?> Ask(TRequest message, IGenericActorRef sender, TimeSpan timeout)
    {
        using CancellationTokenSource timeoutCancellationTokenSource = new();

        TaskCompletionSource<TResponse?> completionSource = Runner.SendAndTryDeliver(message, sender, null);

        Task<TResponse?> task = completionSource.Task;

        Task completedTask = await Task.WhenAny(
            task,
            Task.Delay(timeout, timeoutCancellationTokenSource.Token)
        );

        if (completedTask == task)
        {
            await timeoutCancellationTokenSource.CancelAsync();
            return await task;
        }

        throw new AskTimeoutException($"Timeout after {timeout} waiting for a reply");
    }
}