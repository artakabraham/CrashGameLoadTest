using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Interfaces
{
    public interface IBetDecisionStrategy
    {
        Task<bool> ShouldBetAsync(PlayerContext context, CancellationToken cancellationToken);
        Task<double> GetBetAmountAsync(PlayerContext context, CancellationToken cancellationToken);
    }
}