using CrashGameLoadTest.Core;

namespace CrashGameLoadTest.Strategies
{
    public class RandomBetStrategy : IBetDecisionStrategy
    {
        private readonly Random _random = new();
        private readonly decimal _minBet;
        private readonly decimal _maxBet;
        private readonly double _betProbability;

        public RandomBetStrategy(decimal minBet = 5m, decimal maxBet = 50m, double betProbability = 0.7)
        {
            _minBet = minBet;
            _maxBet = maxBet;
            _betProbability = betProbability;
        }

        public Task<bool> ShouldBetAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            var shouldBet = _random.NextDouble() < _betProbability &&
                            context.Balance >= _minBet &&
                            !context.IsInGame;
            return Task.FromResult(shouldBet);
        }

        public Task<decimal> GetBetAmountAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            var betAmount = (decimal)(_random.NextDouble() * (double)(_maxBet - _minBet)) + _minBet;
            return Task.FromResult(Math.Min(betAmount, context.Balance));
        }
    }
}