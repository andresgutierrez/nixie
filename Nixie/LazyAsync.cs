
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Nixie;

/// <summary>
/// LazyTask utility
/// </summary>
/// <typeparam name="T"></typeparam>
[AsyncMethodBuilder(typeof(LazyTaskMethodBuilder<>))]
public class LazyTask<T> : INotifyCompletion
{
    private readonly Lock syncObj = new();

    private T? result;

    private Exception? exception;

    private IAsyncStateMachine? asyncStateMachine;

    private Action? continuation;

    /// <summary>
    /// Constructor
    /// </summary>
    internal LazyTask()
    {
    }

    /// <summary>
    /// Async state machine get result
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public T? GetResult()
    {
        lock (syncObj)
        {
            if (exception != null)
                ExceptionDispatchInfo.Throw(exception);

            if (!IsCompleted)
                throw new Exception("Not Completed");

            return result;
        }
    }

    /// <summary>
    /// Returns true if the task is completed
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Handler when the task is completed
    /// </summary>
    /// <param name="continuation"></param>
    public void OnCompleted(Action continuation)
    {
        lock (syncObj)
        {
            if (asyncStateMachine != null)
            {
                try
                {
                    asyncStateMachine.MoveNext();
                }
                finally
                {
                    asyncStateMachine = null;
                }
            }

            if (continuation == null)
                this.continuation = continuation;
            else
                this.continuation += continuation;

            TryCallContinuation();
        }
    }

    /// <summary>
    /// Returns the awaiter
    /// </summary>
    /// <returns></returns>
    public LazyTask<T> GetAwaiter() => this;

    internal void SetResult(T result)
    {
        lock (syncObj)
        {
            this.result = result;
            IsCompleted = true;
            TryCallContinuation();
        }
    }

    internal void SetException(Exception exception)
    {
        lock (syncObj)
        {
            this.exception = exception;
            IsCompleted = true;
            TryCallContinuation();
        }
    }

    internal void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        asyncStateMachine = stateMachine;
    }

    private void TryCallContinuation()
    {
        if (IsCompleted && continuation != null)
        {
            try
            {
                continuation();
            }
            finally
            {
                continuation = null;
            }
        }
    }
}
