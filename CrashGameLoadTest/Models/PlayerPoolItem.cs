namespace CrashGameLoadTest.Models
{
    public class PlayerPoolItem
    {
        public string PlayerId { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string B2BToken { get; set; } = string.Empty;
        public string PlatformId { get; set; } = string.Empty;
        public string PartnerId { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public bool IsDemo { get; set; }
        public bool IsInUse { get; set; } = false;
    }
}