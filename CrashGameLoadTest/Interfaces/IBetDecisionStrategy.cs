namespace CrashGameLoadTest.Core
{
    public interface IBetDecisionStrategy
    {
        Task<bool> ShouldBetAsync(PlayerContext context, CancellationToken cancellationToken);
        Task<decimal> GetBetAmountAsync(PlayerContext context, CancellationToken cancellationToken);
    }
}