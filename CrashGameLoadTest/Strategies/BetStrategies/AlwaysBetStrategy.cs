using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Strategies.BetStrategies
{
    public class AlwaysBetStrategy : IBetDecisionStrategy
    {
        private readonly decimal _betAmount;

        public AlwaysBetStrategy(decimal betAmount = 10m)
        {
            _betAmount = betAmount;
        }

        public Task<bool> ShouldBetAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(context.Balance >= _betAmount && !context.IsInGame);
        }

        public Task<decimal> GetBetAmountAsync(PlayerContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(Math.Min(_betAmount, context.Balance));
        }
    }
}