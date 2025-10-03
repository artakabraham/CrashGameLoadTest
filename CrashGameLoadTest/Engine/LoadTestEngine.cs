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

            var playersPerBatch = 2;
            var intervalPerBatchInSeconds = 60;

            var batchCount = (int)Math.Ceiling((double)playerCount / playersPerBatch);

            for (int batch = 0; batch < batchCount; batch++)
            {
                for (var k = 0; k < playersPerBatch; k++)
                {
                    var playerTask = CreateAndRunPlayer(scenario, token);
                    tasks.Add(playerTask);
                    await Task.Delay(TimeSpan.FromSeconds(30), token);
                }
                await Task.Delay(TimeSpan.FromSeconds(60), token);
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