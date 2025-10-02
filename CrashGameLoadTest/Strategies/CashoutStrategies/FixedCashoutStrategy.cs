using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Strategies.CashoutStrategies
{
    public class FixedCashoutStrategy() : ICashoutStrategy
    {
        public Task<bool> ShouldCashoutAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            var random = new Random();
            double min = 1.0;
            double max = 2.5;
            double targetMultiplier = min + (random.NextDouble() * (max - min));

            Console.WriteLine($"[FixedCashoutStrategy] CurrentMultiplier: {context.CurrentMultiplier}, TargetMultiplier: {targetMultiplier}");
            return Task.FromResult(context.IsInGame && context.CurrentMultiplier >= targetMultiplier);
        }
    }
}