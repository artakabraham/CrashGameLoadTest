using CrashGameLoadTest.Game;
using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;
using CrashGameLoadTest.Scenarios;

namespace CrashGameLoadTest.Engine
{
    public class LoadTestEngine
    {
        private readonly ScenarioBuilder _scenarioBuilder;
        private readonly IPlayerFactory _playerFactory;

        public LoadTestEngine(ScenarioBuilder scenarioBuilder, IPlayerFactory playerFactory)
        {
            _scenarioBuilder = scenarioBuilder;
            _playerFactory = playerFactory;
        }

        public async Task RunAsync(int playerCount, string scenarioType, CancellationToken token)
        {
            var scenario = scenarioType.ToLower() switch
            {
                "random" => _scenarioBuilder.CreateRandomScenario(),
                "aggressive" => _scenarioBuilder.CreateAggressiveScenario(),
                _ => _scenarioBuilder.CreateBasicScenario()
            };

            var tasks = new List<Task>();

            Console.WriteLine($"Creating {playerCount} players with {scenario.Name} scenario...");

            for (var i = 0; i < playerCount; i++)
            {
                var playerTask = CreateAndRunPlayer(scenario, token);
                tasks.Add(playerTask);
            }

            Console.WriteLine($"Started {playerCount} players with unique credentials");

            await Task.WhenAll(tasks);

            Console.WriteLine("All players completed");
        }

        private async Task CreateAndRunPlayer(Scenario scenario, CancellationToken token)
        {
            var player = await _playerFactory.CreatePlayerAsync(scenario, token);
            if (player != null)
            {
                await player.RunAsync();
            }
        }
    }
}