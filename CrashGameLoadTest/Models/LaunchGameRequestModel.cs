namespace CrashGameLoadTest.Models
{
    public record LaunchGameRequestModel
    {
        public string B2BToken { get; set; }
        public string PlatformId { get; set; }
        public string PartnerId { get; set; }
        public string PlayerId { get; set; }
        public string Currency { get; set; }
        public string? GameVersion { get; set; }
        public string GameId { get; set; }
        public string Language { get; set; }
        public double Balance { get; set; }
        public string NickName { get; set; }
        public bool IsDemo { get; set; }
    }
}