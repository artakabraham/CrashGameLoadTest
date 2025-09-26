using CrashGameLoadTest.Core;

namespace CrashGameLoadTest.Strategies
{
    public class FixedCashoutStrategy(decimal targetMultiplier = 2.0m) : ICashoutStrategy
    {
        public Task<bool> ShouldCashoutAsync(PlayerContext context, decimal currentMultiplier, CancellationToken cancellationToken)
        {
            return Task.FromResult(context.IsInGame && currentMultiplier >= targetMultiplier);
        }
    }
}