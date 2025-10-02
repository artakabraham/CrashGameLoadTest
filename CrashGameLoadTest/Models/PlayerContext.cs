using CrashGameLoadTest.HttpClient;
using LVC.CrashGamesCore.Domain.Enums;
using Microsoft.AspNetCore.SignalR.Client;

namespace CrashGameLoadTest.Models
{
    public class PlayerContext
    {
        public string PlayerId { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public IntegratorHttpClient? HttpClient { get; set; }
        public HubConnection? SignalRConnection { get; set; }
        public string JwtToken { get; set; } = string.Empty;
        public double Balance { get; set; }
        public double CurrentBet { get; set; }
        public RoundStatusEnum CurrentStatus { get; set; }
        public double CurrentMultiplier { get; set; }
        public double MinBet { get; set; }
        public double MaxBet { get; set; }
        public double MaxWin { get; set; }
        public long BetId { get; set; }
        public Guid RoundId { get; set; }
        public bool IsInGame { get; set; }
        public bool IsCashedOut { get; set; }
        public DateTime LastActionTime { get; set; }
        //public Dictionary<string, object> CustomData { get; set; } = [];
    }
}