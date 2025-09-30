using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Services
{
    public class PlayerPoolService : IPlayerPoolService
    {
        private readonly List<PlayerPoolItem> _playerPool;
        private readonly object _lockObject = new object();

        public PlayerPoolService()
        {
            _playerPool = InitializePlayerPool(100);
        }

        public int AvailablePlayersCount => _playerPool.Count(p => !p.IsInUse);
        public int TotalPlayersCount => _playerPool.Count;

        public PlayerPoolItem? GetAvailablePlayer()
        {
            lock (_lockObject)
            {
                var availablePlayer = _playerPool.FirstOrDefault(p => !p.IsInUse);
                if (availablePlayer != null)
                {
                    availablePlayer.IsInUse = true;
                }

                return availablePlayer;
            }
        }

        public void ReleasePlayer(string playerId)
        {
            lock (_lockObject)
            {
                var player = _playerPool.FirstOrDefault(p => p.PlayerId == playerId);
                if (player != null)
                {
                    player.IsInUse = false;
                }
            }
        }


        private static List<PlayerPoolItem> InitializePlayerPool(int playersCount)
        {
            List<PlayerPoolItem> players = [];

            for (int i = 2; i < playersCount; i++)
            {
                players.Add(
                        new PlayerPoolItem
                        {
                            PlayerId = $"Shant000{i}",
                            NickName = $"Shant000{i}",
                            B2BToken = "string",
                            PlatformId = "1234566666",
                            PartnerId = "01",
                            Currency = "USD",
                            Balance = 10000,
                            IsDemo = false
                        });
            }
            return players;
        }
    }
}