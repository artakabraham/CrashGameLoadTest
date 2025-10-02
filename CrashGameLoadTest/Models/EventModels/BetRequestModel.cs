using LVC.CrashGamesCore.Domain.Enums;

namespace CrashGameLoadTest.Models.EventModels
{
    public class BetRequestModel
    {
        public Guid RoundId { get; set; }
        public BetSectionEnum BetSection { get; set; }
        public double BetAmount { get; set; }
        public double AutoCashoutOdd { get; set; }
        public string Name { get; set; }
    }
}