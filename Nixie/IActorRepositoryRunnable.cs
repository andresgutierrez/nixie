
namespace Nixie;

/// <summary>
/// Represents an actor repository that can be run.
/// </summary>
public interface IActorRepositoryRunnable
{
    /// <summary>
    /// Returns true if there are pending messages to process.
    /// </summary>
    /// <returns></returns>
    public bool HasPendingMessages();

    /// <summary>
    /// Returns true if the repository is processing messages.
    /// </summary>
    /// <returns></returns>
    public bool IsProcessing();
}
