using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;
using CrashGameLoadTest.Strategies.BetStrategies;
using CrashGameLoadTest.Strategies.CashoutStrategies;

namespace CrashGameLoadTest.Scenarios
{
    public class ScenarioBuilder
    {
        public Scenario CreateBasicScenario()
        {
            return new Scenario
            {
                Name = "Basic Scenario",
                Actions = new List<IPlayerAction>(),
                BetStrategy = new AlwaysBetStrategy(10),
                CashoutStrategy = new FixedCashoutStrategy()
            };
        }

        public Scenario CreateRandomScenario()
        {
            return new Scenario
            {
                Name = "Random Scenario",
                Actions = new List<IPlayerAction>(),
                BetStrategy = new RandomBetStrategy(5, 50, 0.6),
                CashoutStrategy = new RandomCashoutStrategy(1.2, 5.0)
            };
        }

        public Scenario CreateAggressiveScenario()
        {
            return new Scenario
            {
                Name = "Aggressive Scenario",
                Actions = new List<IPlayerAction>(),
                BetStrategy = new AlwaysBetStrategy(25),
                CashoutStrategy = new FixedCashoutStrategy()
            };
        }
    }
}