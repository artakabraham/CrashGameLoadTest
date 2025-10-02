using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Strategies.CashoutStrategies
{
    public class RandomCashoutStrategy : ICashoutStrategy
    {
        private readonly Random _random = new();
        private readonly double _minMultiplier;
        private readonly double _maxMultiplier;

        public RandomCashoutStrategy(double minMultiplier = 1.1, double maxMultiplier = 10)
        {
            _minMultiplier = minMultiplier;
            _maxMultiplier = maxMultiplier;
        }

        public Task<bool> ShouldCashoutAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            if (!context.IsInGame || context.CurrentMultiplier < _minMultiplier)
                return Task.FromResult(false);

            // Higher multiplier = higher chance to cashout
            var cashoutProbability = (context.CurrentMultiplier - _minMultiplier) / (_maxMultiplier - _minMultiplier);
            var shouldCashout = _random.NextDouble() < cashoutProbability;

            return Task.FromResult(shouldCashout);
        }
    }
}