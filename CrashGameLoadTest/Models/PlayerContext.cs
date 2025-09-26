using Microsoft.AspNetCore.SignalR.Client;

namespace CrashGameLoadTest.Models
{
    public class PlayerContext
    {
        public string PlayerId { get; set; } = string.Empty;
        public HttpClient HttpClient { get; set; } = new();
        public HubConnection? SignalRConnection { get; set; }
        public string JwtToken { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public decimal CurrentBet { get; set; }
        public decimal CurrentMultiplier { get; set; }
        public bool IsInGame { get; set; }
        public DateTime LastActionTime { get; set; }
        public Dictionary<string, object> CustomData { get; } = new();
    }
}