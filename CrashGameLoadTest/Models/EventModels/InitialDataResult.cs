using LVC.CrashGamesCore.Domain.Enums;

namespace CrashGameLoadTest.Models.EventModels
{

    public class InitialDataResult
    {
        public Game Game { get; set; }
        public Round Round { get; set; }
        public User User { get; set; }
    }

    public class Game
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Language { get; set; }
        public double MinBet { get; set; }
        public double MaxBet { get; set; }
        public double MaxWin { get; set; }
        public bool? IsDemo { get; set; }
        public bool IsMaintenance { get; set; }
        public int? MaintenanceStartDateInSeconds { get; set; }
    }

    public class Round
    {
        public Guid Id { get; set; }
        public long RoundNumber { get; set; }
        public RoundStatusEnum CurrentStatus { get; set; }
        public int Duration { get; set; }
        public int? SecoundDuration { get; set; }
        public double? ElapsedOddTime { get; set; }
        public DateTime? StartOddTime { get; set; }
        public double? CrashPoint { get; set; }
        public double? BonusBankAmount { get; set; }
    }

    public class User
    {
        public Guid UserId { get; set; }
        public Guid PlayerId { get; set; }
        public long DisplayId { get; set; }
        public string Currency { get; set; }
        public BonusBankWinData? BonusBankWinData { get; set; }
        public double Balance { get; set; }
        public double Rate { get; set; }
    }

    public class BonusBankWinData
    {
        public double? BankWin { get; set; }
        public BetSectionEnum BetSection { get; set; }
    }
}