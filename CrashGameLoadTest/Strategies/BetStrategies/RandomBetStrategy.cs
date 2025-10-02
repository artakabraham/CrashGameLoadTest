using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Strategies.BetStrategies
{
    public class RandomBetStrategy : IBetDecisionStrategy
    {
        private readonly Random _random = new();
        private readonly double _minBet;
        private readonly double _maxBet;
        private readonly double _betProbability;

        public RandomBetStrategy(double minBet = 5, double maxBet = 50, double betProbability = 0.7)
        {
            _minBet = minBet;
            _maxBet = maxBet;
            _betProbability = betProbability;
        }

        public Task<bool> ShouldBetAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            var shouldBet = _random.NextDouble() < _betProbability && context.Balance >= _minBet && !context.IsInGame;
            return Task.FromResult(shouldBet);
        }

        public Task<double> GetBetAmountAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            var betAmount = (_random.NextDouble() * (_maxBet - _minBet)) + _minBet;
            return Task.FromResult(Math.Min(betAmount, context.Balance));
        }
    }
}