
using System.Runtime.CompilerServices;

namespace Nixie;

public class LazyTaskMethodBuilder<T>
{
    public LazyTaskMethodBuilder() => Task = new LazyTask<T>();

    //public static LazyTaskMethodBuilder<T> Create() => new();

    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
    {        
        Task.SetStateMachine(stateMachine);
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine) { }

    public void SetException(Exception exception) => this.Task.SetException(exception);

    public void SetResult(T result) => this.Task.SetResult(result);

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
        =>
            GenericAwaitOnCompleted(ref awaiter, ref stateMachine);

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
        =>
            GenericAwaitOnCompleted(ref awaiter, ref stateMachine);

    public void GenericAwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        awaiter.OnCompleted(stateMachine.MoveNext);
    }

    public LazyTask<T> Task { get; }
}
