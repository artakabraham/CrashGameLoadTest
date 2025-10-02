using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Strategies.CashoutStrategies
{
    public class FixedCashoutStrategy() : ICashoutStrategy
    {
        public Task<bool> ShouldCashoutAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            var random = new Random();
            double min = 1.5;
            double max = 15.0;
            double targetMultiplier = min + (random.NextDouble() * (max - min));

            return Task.FromResult(context.IsInGame && context.CurrentMultiplier >= targetMultiplier);
        }
    }
}