using LVC.CrashGamesCore.Domain.Enums;

namespace CrashGameLoadTest.Models.EventModels
{
    public class DoBetRequestModel
    {
        public Guid RoundId { get; set; }
        public BetSectionEnum BetSection { get; set; }
        public decimal BetAmount { get; set; }
        public double AutoCashoutOdd { get; set; }
        public string Name { get; set; }
    }
}