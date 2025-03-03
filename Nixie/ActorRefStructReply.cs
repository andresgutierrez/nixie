
using DotNext.Threading.Tasks;

namespace Nixie;

/// <summary>
/// Represents an actor reference.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ActorRefStruct<TActor, TRequest, TResponse> : IGenericActorRef, IActorRefStruct<TActor, TRequest, TResponse>
    where TActor : IActorStruct<TRequest, TResponse> where TRequest : struct where TResponse : struct
{
    private readonly ActorRunnerStruct<TActor, TRequest, TResponse> runner;

    /// <summary>
    /// Returns a reference to the actor's runner
    /// </summary>
    public ActorRunnerStruct<TActor, TRequest, TResponse> Runner => runner;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="runner"></param>
    public ActorRefStruct(ActorRunnerStruct<TActor, TRequest, TResponse> runner)
    {
        this.runner = runner;
    }

    /// <summary>
    /// Passes a message to the actor without expecting a response and without specifying a sender
    /// </summary>
    /// <param name="message"></param>
    public void Send(TRequest message)
    {
        runner.SendAndTryDeliver(message, null, null);
    }

    /// <summary>
    /// Passes a message to the actor without expecting a response and specifying a sender
    /// </summary>
    /// <param name="message"></param>
    /// <param name="sender"></param>
    public void Send(TRequest message, IGenericActorRef sender)
    {
        runner.SendAndTryDeliver(message, sender, null);
    }

    /// <summary>
    /// Passes a message to the actor without expecting a response and specifying a parent promise
    /// </summary>
    /// <param name="message"></param>
    /// <param name="parentPromise"></param>
    public void Send(TRequest message, ActorMessageReply<TRequest, TResponse>? parentPromise)
    {
        runner.SendAndTryDeliver(message, null, parentPromise);
    }

    /// <summary>
    /// Sends a message to actor expecting a response and without specifying a sender
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>    
    public async Task<TResponse> Ask(TRequest message)
    {
        ValueTaskCompletionSource<TResponse> promise = runner.SendAndTryDeliver(message, null, null);

        return await promise.CreateTask(TimeSpan.FromHours(1), CancellationToken.None);
    }

    /// <summary>
    /// Sends a message to the actor and expects a response
    /// An exception will be thrown if the timeout limit is reached
    /// </summary>
    /// <param name="message"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="AskTimeoutException"></exception>
    public async Task<TResponse> Ask(TRequest message, TimeSpan timeout)
    {
        using CancellationTokenSource timeoutCancellationTokenSource = new();

        ValueTaskCompletionSource<TResponse> completionSource = runner.SendAndTryDeliver(message, null, null);

        Task<TResponse> task = completionSource.CreateTask(TimeSpan.FromHours(1), CancellationToken.None).AsTask();
        
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
    public async Task<TResponse> Ask(TRequest message, IGenericActorRef sender)
    {
        ValueTaskCompletionSource<TResponse> promise = runner.SendAndTryDeliver(message, sender, null);

        return await promise.CreateTask(TimeSpan.FromHours(1), default);
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
    public async Task<TResponse> Ask(TRequest message, IGenericActorRef sender, TimeSpan timeout)
    {
        using CancellationTokenSource timeoutCancellationTokenSource = new();

        ValueTaskCompletionSource<TResponse> completionSource = runner.SendAndTryDeliver(message, sender, null);

        Task<TResponse> task = completionSource.CreateTask(TimeSpan.Zero, default).AsTask();

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