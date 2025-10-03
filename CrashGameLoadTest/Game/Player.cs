using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;
using CrashGameLoadTest.Models.EventModels;
using LVC.CrashGamesCore.Domain.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrashGameLoadTest.Game
{
    public class Player
    {
        private readonly Scenario _scenario;
        private readonly PlayerContext _playerContext;
        private readonly PlayerPoolItem _poolPlayer;
        private readonly IPlayerPoolService _playerPoolService;
        private readonly CancellationToken _cancellationToken;
        public static ConcurrentDictionary<Guid, BetData> PlayerGameState { get; set; } = [];

        public Player(
            Scenario scenario,
            PlayerContext context,
            PlayerPoolItem poolPlayer,
            IPlayerPoolService playerPoolService,
            CancellationToken cancellationToken)
        {
            _scenario = scenario;
            _playerContext = context;
            _poolPlayer = poolPlayer;
            _playerPoolService = playerPoolService;
            _cancellationToken = cancellationToken;
        }

        public async Task RunAsync()
        {
            try
            {
                Console.WriteLine($"Player {_playerContext.PlayerId} starting");

                await ConnectToHub();

                while (!_cancellationToken.IsCancellationRequested)
                {
                    //await PlayGameRound();
                    //await Task.Delay(1000, _cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Player {_playerContext.PlayerId} cancelled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Player {_playerContext.PlayerId} error: {ex.Message}");
            }
            finally
            {
                await CleanupAsync();
            }
        }

        private async Task ConnectToHub()
        {
            var hubUrl = $"wss://crashaxy.tst.rbtplay.net/hubs/lvccrashaxy?gameId=7&access_token={_playerContext.JwtToken}";

            _playerContext.SignalRConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            RegisterEventHandlers();

            await _playerContext.SignalRConnection.StartAsync(_cancellationToken);

            if (_playerContext.SignalRConnection != null)
            {
                await _playerContext.SignalRConnection.SendAsync("GetInitialData", _cancellationToken);
            }

            Console.WriteLine($"Player {_playerContext.PlayerId} connected to SignalR hub");
        }

        private void RegisterEventHandlers()
        {
            _playerContext?.SignalRConnection?.On<object>("OnConnected", OnConnected);
            _playerContext?.SignalRConnection?.On<object>("InitialDataResult", OnInitialDataResult);
            _playerContext?.SignalRConnection?.On<object>("ResultReport", OnResultReport);
            _playerContext?.SignalRConnection?.On<object>("EndRound", OnEndRound);
            _playerContext?.SignalRConnection?.On<object>("CreateRound", OnCreateRound);
            _playerContext?.SignalRConnection?.On<object>("BetsClosed", OnBetsClosed);
            _playerContext?.SignalRConnection?.On<object>("DoBetResult", PlacedBetResult);
            //_playerContext.SignalRConnection.On<object>("UserBalance", UserBalance);
            //_playerContext.SignalRConnection.On<object>("RoundResult", OnRoundResult);
        }

        private void OnConnected(object data)
        {
            Console.WriteLine($"Player {_playerContext.PlayerId} - Connected to hub");
            Console.WriteLine("OnConnected", data);
        }

        private async Task OnInitialDataResult(object data)
        {
            if (_playerContext.SignalRConnection != null)
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

                PlayerGameState.TryAdd(initialData.User.UserId, new BetData { UserId = initialData.User.UserId });

                _playerContext.UserId = initialData.User.UserId;
                _playerContext.Balance = initialData.User.Balance;
                _playerContext.MinBet = initialData.Game.MinBet;
                _playerContext.MaxBet = initialData.Game.MaxBet;
                _playerContext.MaxWin = initialData.Game.MaxWin;
                _playerContext.CurrentStatus = initialData.Round.CurrentStatus;
                Console.WriteLine($"InitialDataResult -  {initialData.User.UserId}");
            }
        }

        private async Task OnResultReport(object data)
        {
            if (_playerContext.SignalRConnection != null)
            {
                var jsonString = data is JsonElement element
                ? element.GetRawText()
                : data.ToString();

                var resultReportData = JsonSerializer.Deserialize<ResultReportModel>(
                    jsonString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    });

                _playerContext.CurrentMultiplier = resultReportData.Odd;

                // TODO Check Game State before calling Cashout
                await Cashout(_playerContext.RoundId);

                Console.WriteLine($"ResultReport - {data}");
            }
        }

        private async Task OnCreateRound(object data)
        {
            if (_playerContext.SignalRConnection != null)
            {
                var jsonString = data is JsonElement element
                ? element.GetRawText()
                : data.ToString();

                var createRoundData = JsonSerializer.Deserialize<CreateRound>(
                    jsonString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    });

                _playerContext.RoundId = createRoundData.Id;

                await PlayGameRound(createRoundData.Id);

                Console.WriteLine($"CreateRound - {createRoundData.Id}");
            }
        }

        private async Task PlayGameRound(Guid roundId)
        {
            if (_scenario.BetStrategy != null && await _scenario.BetStrategy.ShouldBetAsync(_playerContext, _cancellationToken))
            {
                var betAmount = await _scenario.BetStrategy.GetBetAmountAsync(_playerContext, _cancellationToken);

                var doBetModel = new BetRequestModel
                {
                    RoundId = roundId,
                    BetSection = BetSectionEnum.BetSectionLeft,
                    BetAmount = betAmount,
                    Name = "Crashaxy"
                };

                await PlaceBet(doBetModel);
            }
        }

        private async Task PlaceBet(BetRequestModel doBetRequestModel)
        {
            if (_playerContext.SignalRConnection != null)
            {
                var random = new Random();
                var delay = (random.Next(3) * 1000); // Random delay between 1 and 5 seconds

                Console.WriteLine($"Delay - {delay}");

                await Task.Delay(delay, _cancellationToken);

                await _playerContext.SignalRConnection.SendAsync("DoBet", doBetRequestModel, _cancellationToken);
            }
        }


        private async Task PlacedBetResult(object data)
        {
            if (_playerContext.SignalRConnection != null)
            {
                var jsonString = data is JsonElement element
                    ? element.GetRawText()
                    : data.ToString();
                var doBetResultData = JsonSerializer.Deserialize<BetResultModel>(
                    jsonString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    });

                PlayerGameState[doBetResultData.UserId].BetId = doBetResultData.BetId;

                _playerContext.BetId = doBetResultData.BetId;
                _playerContext.IsInGame = true;
            }
        }

        private async Task OnBetsClosed(object data)
        {
            if (_playerContext.SignalRConnection != null)
            {
                Console.WriteLine($"BetsClosed - {data}");
            }
        }

        private async Task Cashout(Guid roundId)
        {
            if (roundId == Guid.Empty)
            {
                return;
            }

            if (_playerContext.IsInGame && _scenario.CashoutStrategy != null)
            {
                if (await _scenario.CashoutStrategy.ShouldCashoutAsync(_playerContext, _cancellationToken))
                {
                    var cashOutRequest = new CashoutRequestModel
                    {
                        Odd = _playerContext.CurrentMultiplier,
                        RoundId = roundId,
                        BetSection = BetSectionEnum.BetSectionLeft,
                        IsHalfCashout = false,
                        BetId = PlayerGameState[_playerContext.UserId].BetId,
                        CashoutTime = DateTime.UtcNow
                    };

                    if (_playerContext.SignalRConnection != null)
                    {
                        await _playerContext.SignalRConnection.SendAsync("DoCashout", cashOutRequest, _cancellationToken);
                        Console.WriteLine($"Player {_playerContext.PlayerId} cashed out, BetId = {cashOutRequest.BetId}");
                    }
                }
            }
        }
        private async Task OnEndRound(object data)
        {
            if (_playerContext.SignalRConnection != null)
            {
                Console.WriteLine($"EndRound - {data}");
                //CustomData.TryRemove(_playerContext.PlayerId, out _);
                _playerContext.IsInGame = false;
                _playerContext.BetId = 0;
            }
        }

        private async Task CleanupAsync()
        {
            if (_playerContext.SignalRConnection != null)
            {
                await _playerContext.SignalRConnection.DisposeAsync();
            }

            _playerPoolService.ReleasePlayer(_poolPlayer.PlayerId);
            _playerContext.IsInGame = false;
            Console.WriteLine($"Player {_playerContext.PlayerId} released back to pool");
        }
    }

    public class BetData
    {
        public Guid UserId { get; set; }
        public long BetId { get; set; }
    }
}