using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Strategies.CashoutStrategies
{
    public class FixedCashoutStrategy(decimal targetMultiplier = 2.0m) : ICashoutStrategy
    {
        public Task<bool> ShouldCashoutAsync(PlayerContext context, decimal currentMultiplier, CancellationToken cancellationToken)
        {
            return Task.FromResult(context.IsInGame && currentMultiplier >= targetMultiplier);
        }
    }
}