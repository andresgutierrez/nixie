
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
    public bool HasPendingMessages(out string? actorName);

    /// <summary>
    /// Returns true if the repository is processing messages.
    /// </summary>
    /// <returns></returns>
    public bool IsProcessing(out string? actorName);
}
