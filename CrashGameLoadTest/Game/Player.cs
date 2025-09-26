using CrashGameLoadTest.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace CrashGameLoadTest.Game
{
    public class Player
    {
        private readonly Scenario _scenario;
        private readonly PlayerContext _context;
        private readonly CancellationToken _cancellationToken;
        
        public Player(Scenario scenario, PlayerContext context, CancellationToken cancellationToken)
        {
            _scenario = scenario;
            _context = context;
            _cancellationToken = cancellationToken;
        }
        
        public async Task RunAsync()
        {
            try
            {
                // Execute initial actions (auth, connect)
                foreach (var action in _scenario.Actions)
                {
                    await action.ExecuteAsync(_context, _cancellationToken);
                }

                // Set initial balance
                _context.Balance = 1000m; // Starting balance

                // Main game loop
                while (!_cancellationToken.IsCancellationRequested)
                {
                    await PlayGameRound();
                    await Task.Delay(1000, _cancellationToken); // Wait between rounds
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            finally
            {
                //await CleanupAsync();
            }
        }

        private async Task PlayGameRound()
        {
            if (_scenario.BetStrategy != null && await _scenario.BetStrategy.ShouldBetAsync(_context, _cancellationToken))
            {
                var betAmount = await _scenario.BetStrategy.GetBetAmountAsync(_context, _cancellationToken);
                await PlaceBet(betAmount);
            }

            if (_context.IsInGame && _scenario.CashoutStrategy != null)
            {
                if (await _scenario.CashoutStrategy.ShouldCashoutAsync(_context, _context.CurrentMultiplier, _cancellationToken))
                {
                    await Cashout();
                }
            }
        }

        private async Task PlaceBet(decimal amount)
        {
            _context.CurrentBet = amount;
            _context.Balance -= amount;
            _context.IsInGame = true;
            _context.CurrentMultiplier = 1.0m;
            _context.LastActionTime = DateTime.UtcNow;

            // Simulate bet placement via SignalR
            if (_context.SignalRConnection != null)
            {
                await _context.SignalRConnection.SendAsync("PlaceBet", amount, _cancellationToken);
            }
        }

        private async Task Cashout()
        {
            var winnings = _context.CurrentBet * _context.CurrentMultiplier;
            _context.Balance += winnings;
            _context.IsInGame = false;
            _context.CurrentBet = 0;
            _context.CurrentMultiplier = 1.0m;
            _context.LastActionTime = DateTime.UtcNow;

            // Simulate cashout via SignalR
            if (_context.SignalRConnection != null)
            {
                await _context.SignalRConnection.SendAsync("Cashout", _cancellationToken);
            }
        }

        // private async Task CleanupAsync()
        // {
        //     if (_context.SignalRConnection != null)
        //     {
        //         await _context.SignalRConnection.DisposeAsync();
        //     }
        //     _context.HttpClient?.Dispose();
        // }
    }
}