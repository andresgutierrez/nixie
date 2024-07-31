
namespace Nixie;

/// <summary>
/// This interface must be implemented by all actors that do not return a response.
/// This actor type supports struct request messages.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IActorStruct<in TRequest> where TRequest : struct
{
    public Task Receive(TRequest message);
}
