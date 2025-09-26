using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Core
{
    public interface ICashoutStrategy
    {
        Task<bool> ShouldCashoutAsync(PlayerContext context, decimal currentMultiplier, CancellationToken cancellationToken);
    }
}