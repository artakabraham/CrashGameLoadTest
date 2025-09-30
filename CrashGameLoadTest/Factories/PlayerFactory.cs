using CrashGameLoadTest.Game;
using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Factories
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IPlayerPoolService _playerPoolService;
        private readonly ILaunchGameService _launchGameService;

        public PlayerFactory(IPlayerPoolService playerPoolService, ILaunchGameService launchGameService)
        {
            _playerPoolService = playerPoolService;
            _launchGameService = launchGameService;
        }

        public async Task<Player?> CreatePlayerAsync(Scenario scenario, CancellationToken cancellationToken)
        {
            var poolPlayer = _playerPoolService.GetAvailablePlayer();
            if (poolPlayer == null)
            {
                Console.WriteLine("No available players in pool");
                return null;
            }

            try
            {
                var jwtToken = await _launchGameService.LaunchGameAsync(poolPlayer, cancellationToken);

                var playerContext = new PlayerContext
                {
                    PlayerId = poolPlayer.PlayerId,
                    JwtToken = jwtToken,
                    Balance = poolPlayer.Balance
                };

                return new Player(scenario, playerContext, poolPlayer, _playerPoolService, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create player {poolPlayer.PlayerId}: {ex.Message}");
                _playerPoolService.ReleasePlayer(poolPlayer.PlayerId);
                return null;
            }
        }
    }
}