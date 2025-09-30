using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Interfaces
{
    public interface IPlayerPoolService
    {
        PlayerPoolItem? GetAvailablePlayer();
        void ReleasePlayer(string playerId);
        int AvailablePlayersCount { get; }
        int TotalPlayersCount { get; }
    }
}