
namespace Nyx;

public interface IActorRepositoryRunnable
{
    public bool HasPendingMessages();

    public Task Run();
}
