using LVC.CrashGamesCore.Domain.Enums;

namespace CrashGameLoadTest.Models.EventModels
{
    public class ResultReportModel
    {
        public required Guid Id { get; set; }
        public required long DisplayId { get; set; }
        public required RoundStatusEnum RoundStatus { get; set; }
        public required long RoundNumber { get; set; }
        public bool IsCrash { get; set; }
        public double Odd { get; set; }
    }
}