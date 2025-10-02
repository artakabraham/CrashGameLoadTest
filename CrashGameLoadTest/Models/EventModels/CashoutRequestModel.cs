using LVC.CrashGamesCore.Domain.Enums;

namespace CrashGameLoadTest.Models.EventModels
{
    public class CashoutRequestModel
    {
        public double Odd { get; set; }
        public Guid RoundId { get; set; }
        public BetSectionEnum BetSection { get; set; }
        public bool IsHalfCashout { get; set; }
        public long BetId { get; set; }
        public DateTime CashoutTime { get; set; }
    }
}
