
namespace Nixie;

public interface IActorRepositoryRunnable
{
    public bool HasPendingMessages();

    public bool IsProcessing();
}
