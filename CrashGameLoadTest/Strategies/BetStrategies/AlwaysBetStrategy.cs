using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Strategies.BetStrategies
{
    public class AlwaysBetStrategy : IBetDecisionStrategy
    {
        private readonly double _betAmount;

        public AlwaysBetStrategy(double betAmount = 10)
        {
            _betAmount = betAmount;
        }

        public Task<bool> ShouldBetAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(context.Balance >= _betAmount && context.IsInGame);
        }

        public Task<double> GetBetAmountAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(Math.Min(_betAmount, context.Balance));
        }
    }
}