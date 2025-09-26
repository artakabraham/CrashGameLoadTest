using CrashGameLoadTest.Game;
using CrashGameLoadTest.Models;
using CrashGameLoadTest.Scenarios;

namespace CrashGameLoadTest.Engine
{
    public class LoadTestEngine
    {
        private readonly ScenarioBuilder _scenarioBuilder;
        
        public LoadTestEngine(ScenarioBuilder scenarioBuilder)
        {
            _scenarioBuilder = scenarioBuilder;
        }
        
        public async Task RunAsync(int playerCount, string scenarioType, CancellationToken token)
        {
            var scenario = scenarioType.ToLower() switch
            {
                "random" => _scenarioBuilder.CreateRandomScenario(),
                "aggressive" => _scenarioBuilder.CreateAggressiveScenario(),
                _ => _scenarioBuilder.CreateBasicScenario()
            };

            var players = new List<Player>();
            var tasks = new List<Task>();

            for (int i = 0; i < playerCount; i++)
            {
                var context = new PlayerContext { PlayerId = $"Player_{i}" };
                var player = new Player(scenario, context, token);
                players.Add(player);
                tasks.Add(player.RunAsync());
            }

            Console.WriteLine($"Started {playerCount} players with {scenario.Name} scenario");
            
            await Task.WhenAll(tasks);
            
            Console.WriteLine("All players completed");
        }
    }
}