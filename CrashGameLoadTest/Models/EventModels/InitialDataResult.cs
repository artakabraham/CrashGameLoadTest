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
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public decimal MaxWin { get; set; }
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
        public decimal? BonusBankAmount { get; set; }
    }

    public class User
    {
        public Guid UserId { get; set; }
        public Guid PlayerId { get; set; }
        public long DisplayId { get; set; }
        public string Currency { get; set; }
        public BonusBankWinData? BonusBankWinData { get; set; }
        public decimal Balance { get; set; }
        public decimal Rate { get; set; }
    }

    public class BonusBankWinData
    {
        public decimal? BankWin { get; set; }
        public BetSectionEnum BetSection { get; set; }
    }
}