using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Core
{
    public interface IPlayerAction
    {
        Task ExecuteAsync(PlayerContext context, CancellationToken cancellationToken);
    }
}