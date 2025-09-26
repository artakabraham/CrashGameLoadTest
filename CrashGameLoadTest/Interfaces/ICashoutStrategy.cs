using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Interfaces
{
    public interface ICashoutStrategy
    {
        Task<bool> ShouldCashoutAsync(PlayerContext context, decimal currentMultiplier, CancellationToken cancellationToken);
    }
}