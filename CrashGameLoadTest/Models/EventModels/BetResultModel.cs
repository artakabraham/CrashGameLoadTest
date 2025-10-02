using LVC.SharedModels.Enums.Bet;

namespace CrashGameLoadTest.Models.EventModels
{
    public class BetResultModel
    {
        public Guid RoundId { get; set; }
        public decimal Amount { get; set; }
        public decimal Coin { get; set; }
        public BetStatusEnum BetStatus { get; set; }
        public double AutoCashOutOdd { get; set; }
        public decimal? WinAmount { get; set; }
        public decimal? WinCoin { get; set; }
        public decimal? HalfWinAmount { get; set; }
        public decimal? HalfWinCoin { get; set; }
        public double? Odd { get; set; }
        public double? HalfOdd { get; set; }
        public DateTime CreatedDate { get; set; }
        public long BetId { get; set; }
        public long? DisplayId { get; set; }
        public decimal Rate { get; set; }
    }
}
