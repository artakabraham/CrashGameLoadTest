using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Interfaces
{
    public interface IPlayerAction
    {
        Task ExecuteAsync(PlayerContext context, CancellationToken cancellationToken);
    }
}