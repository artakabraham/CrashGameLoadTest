using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Strategies.CashoutStrategies
{
    public class RandomCashoutStrategy : ICashoutStrategy
    {
        private readonly Random _random = new();
        private readonly decimal _minMultiplier;
        private readonly decimal _maxMultiplier;

        public RandomCashoutStrategy(decimal minMultiplier = 1.1m, decimal maxMultiplier = 10.0m)
        {
            _minMultiplier = minMultiplier;
            _maxMultiplier = maxMultiplier;
        }

        public Task<bool> ShouldCashoutAsync(PlayerContext context, decimal currentMultiplier, CancellationToken cancellationToken)
        {
            if (!context.IsInGame || currentMultiplier < _minMultiplier)
                return Task.FromResult(false);

            // Higher multiplier = higher chance to cashout
            var cashoutProbability = (double)(currentMultiplier - _minMultiplier) / (double)(_maxMultiplier - _minMultiplier);
            var shouldCashout = _random.NextDouble() < cashoutProbability;

            return Task.FromResult(shouldCashout);
        }
    }
}