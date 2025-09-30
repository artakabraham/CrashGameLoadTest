using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Interfaces
{
    public interface ILaunchGameService
    {
        Task<string> LaunchGameAsync(PlayerPoolItem player, CancellationToken cancellationToken);
    }
}