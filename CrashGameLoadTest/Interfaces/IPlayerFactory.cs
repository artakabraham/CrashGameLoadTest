using CrashGameLoadTest.Game;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Interfaces
{
    public interface IPlayerFactory
    {
        Task<Player?> CreatePlayerAsync(Scenario scenario, CancellationToken cancellationToken);
    }
}