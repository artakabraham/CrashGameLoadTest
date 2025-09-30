using LVC.CrashGamesCore.Domain.Enums;

namespace CrashGameLoadTest.Models.EventModels
{
    public class CreateRound
    {
        public Guid Id { get; set; }
        public long DisplayId { get; set; }
        public long RoundNumber { get; set; }
        public RoundStatusEnum Status { get; set; }
        public int BettingTime { get; set; }
    }

}