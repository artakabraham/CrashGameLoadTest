using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Interfaces
{
    public interface ICashoutStrategy
    {
        Task<bool> ShouldCashoutAsync(PlayerContext context, CancellationToken cancellationToken);
    }
}