namespace CrashGameLoadTest.Models.EventModels
{
    public class SignalRMessage<T>
    {
        public int Type { get; set; }
        public string Target { get; set; } = string.Empty;
        public List<T> Arguments { get; set; } = [];
        public string InvocationId { get; set; } = string.Empty;
    }
}