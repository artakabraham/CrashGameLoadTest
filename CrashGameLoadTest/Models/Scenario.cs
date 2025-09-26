using CrashGameLoadTest.Interfaces;

namespace CrashGameLoadTest.Models
{
    public class Scenario
    {
        public string Name { get; set; } = string.Empty;
        public List<IPlayerAction> Actions { get; set; } = [];
        public IBetDecisionStrategy? BetStrategy { get; set; }
        public ICashoutStrategy? CashoutStrategy { get; set; }
    }
}