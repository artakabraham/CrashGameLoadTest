using CrashGameLoadTest.Actions;
using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;
using CrashGameLoadTest.Strategies.BetStrategies;
using CrashGameLoadTest.Strategies.CashoutStrategies;

namespace CrashGameLoadTest.Scenarios
{
    public class ScenarioBuilder
    {
        private readonly string _integratorUrl;
        private readonly string _hubUrl;
        private readonly string _username;
        private readonly string _password;

        public ScenarioBuilder(string integratorUrl, string hubUrl, string username, string password)
        {
            _integratorUrl = integratorUrl;
            _hubUrl = hubUrl;
            _username = username;
            _password = password;
        }
        
        public Scenario CreateBasicScenario()
        {
            return new Scenario
            {
                Name = "Basic Scenario",
                Actions = new List<IPlayerAction>
                {
                    new AuthenticateAction(_integratorUrl, _username, _password),
                    new ConnectSignalRAction(_hubUrl)
                },
                BetStrategy = new AlwaysBetStrategy(10m),
                CashoutStrategy = new FixedCashoutStrategy(2.0m)
            };
        }
        
        public Scenario CreateRandomScenario()
        {
            return new Scenario
            {
                Name = "Random Scenario",
                Actions = new List<IPlayerAction>
                {
                    new AuthenticateAction(_integratorUrl, _username, _password),
                    new ConnectSignalRAction(_hubUrl)
                },
                BetStrategy = new RandomBetStrategy(5m, 50m, 0.6),
                CashoutStrategy = new RandomCashoutStrategy(1.2m, 5.0m)
            };
        }
        
        public Scenario CreateAggressiveScenario()
        {
            return new Scenario
            {
                Name = "Aggressive Scenario",
                Actions = new List<IPlayerAction>
                {
                    new AuthenticateAction(_integratorUrl, _username, _password),
                    new ConnectSignalRAction(_hubUrl)
                },
                BetStrategy = new AlwaysBetStrategy(25m),
                CashoutStrategy = new FixedCashoutStrategy(1.5m)
            };
        }
        
    }
}