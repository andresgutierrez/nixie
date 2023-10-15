
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Nyx;

[AsyncMethodBuilder(typeof(LazyTaskMethodBuilder<>))]
public class LazyTask<T> : INotifyCompletion
{
    private readonly object syncObj = new();

    private T? result;

    private Exception? exception;

    private IAsyncStateMachine? asyncStateMachine;

    private Action? continuation;

    internal LazyTask()
    {
    }

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

    public bool IsCompleted { get; private set; }

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
