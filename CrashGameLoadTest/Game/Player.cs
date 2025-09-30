using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;
using CrashGameLoadTest.Models.EventModels;
using LVC.CrashGamesCore.Domain.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrashGameLoadTest.Game
{
    public class Player
    {
        private readonly Scenario _scenario;
        private readonly PlayerContext _context;
        private readonly PlayerPoolItem _poolPlayer;
        private readonly IPlayerPoolService _playerPoolService;
        private readonly CancellationToken _cancellationToken;

        public Player(
            Scenario scenario,
            PlayerContext context,
            PlayerPoolItem poolPlayer,
            IPlayerPoolService playerPoolService,
            CancellationToken cancellationToken)
        {
            _scenario = scenario;
            _context = context;
            _poolPlayer = poolPlayer;
            _playerPoolService = playerPoolService;
            _cancellationToken = cancellationToken;
        }

        public async Task RunAsync()
        {
            try
            {
                Console.WriteLine($"Player {_context.PlayerId} starting");

                await ConnectToHub();

                //Main game loop
                while (!_cancellationToken.IsCancellationRequested)
                {
                    //await PlayGameRound();
                    //await Task.Delay(1000, _cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Player {_context.PlayerId} cancelled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Player {_context.PlayerId} error: {ex.Message}");
            }
            finally
            {
                await CleanupAsync();
            }
        }

        private async Task ConnectToHub()
        {
            var hubUrl = $"wss://crashaxy.tst.rbtplay.net/hubs/lvccrashaxy?gameId=7&access_token={_context.JwtToken}";

            _context.SignalRConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            RegisterEventHandlers();

            await _context.SignalRConnection.StartAsync(_cancellationToken);


            if (_context.SignalRConnection != null)
            {
                await _context.SignalRConnection.SendAsync("GetInitialData", _cancellationToken);
                Console.WriteLine("GetInitialData");
            }


            Console.WriteLine($"Player {_context.PlayerId} connected to SignalR hub");
        }

        private void RegisterEventHandlers()
        {
            _context.SignalRConnection.On<object>("OnConnected", OnConnected);
            _context.SignalRConnection.On<object>("InitialDataResult", OnInitialDataResult);
            //_context.SignalRConnection.On<object>("InitialDataResult", InitialDataResult);
            //_context.SignalRConnection.On<object>("RoundResult", OnRoundResult);
            _context.SignalRConnection.On<object>("ResultReport", OnResultReport);
            _context.SignalRConnection.On<object>("EndRound", OnEndRound);
            _context.SignalRConnection.On<object>("CreateRound", OnCreateRound);
            _context.SignalRConnection.On<object>("BetsClosed", OnBetsClosed);
            //_context.SignalRConnection.On<object>("DoBet", OnDoBet);
            //_context.SignalRConnection.On<object>("DoBetResult", DoBetResult);
            //_context.SignalRConnection.On<object>("UserBalance", UserBalance);            
            // Add more events as needed
        }

        private void OnConnected(object data)
        {
            Console.WriteLine($"Player {_context.PlayerId} - Connected to hub");
            Console.WriteLine("OnConnected", data);
        }

        private async Task OnInitialDataResult(object data)
        {
            if (_context.SignalRConnection != null)
            {
                var jsonString = data is JsonElement element
                ? element.GetRawText()
                : data.ToString();

                var initialData = JsonSerializer.Deserialize<InitialDataResult>(
                    jsonString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    });
                Console.WriteLine($"InitialDataResult -  {data}");
            }
        }

        private async Task OnResultReport(object data)
        {
            if (_context.SignalRConnection != null)
            {
                Console.WriteLine($"ResultReport - {data}");
            }
        }

        private async Task OnEndRound(object data)
        {
            if (_context.SignalRConnection != null)
            {
                Console.WriteLine($"EndRound - {data}");
            }
        }

        private async Task OnCreateRound(object data)
        {
            if (_context.SignalRConnection != null)
            {
                var jsonString = data is JsonElement element
                ? element.GetRawText()
                : data.ToString();

                var initialData = JsonSerializer.Deserialize<CreateRound>(
                    jsonString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    });

                var doBetModel = new DoBetRequestModel
                {
                    RoundId = initialData.Id,
                    BetSection = BetSectionEnum.BetSectionLeft,
                    BetAmount = 10,
                    Name = "Crashaxy"
                };

                await OnDoBet(doBetModel);

                Console.WriteLine($"CreateRound - {data}");
            }
        }

        private async Task OnBetsClosed(object data)
        {
            if (_context.SignalRConnection != null)
            {
                Console.WriteLine($"BetsClosed - {data}");
            }
        }

        private async Task OnDoBet(DoBetRequestModel doBetRequestModel)
        {
            if (_context.SignalRConnection != null)
            {
                await Task.Delay(1000, _cancellationToken);

                await _context.SignalRConnection.SendAsync("DoBet", doBetRequestModel, _cancellationToken);
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

            if (_context.SignalRConnection != null)
            {
                await _context.SignalRConnection.SendAsync("PlaceBet", amount, _cancellationToken);
                Console.WriteLine($"Player {_context.PlayerId} placed bet: {amount}");
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

            if (_context.SignalRConnection != null)
            {
                await _context.SignalRConnection.SendAsync("Cashout", _cancellationToken);
                Console.WriteLine($"Player {_context.PlayerId} cashed out: {winnings}");
            }
        }

        private async Task CleanupAsync()
        {
            if (_context.SignalRConnection != null)
            {
                await _context.SignalRConnection.DisposeAsync();
            }

            _playerPoolService.ReleasePlayer(_poolPlayer.PlayerId);
            Console.WriteLine($"Player {_context.PlayerId} released back to pool");
        }
    }
}