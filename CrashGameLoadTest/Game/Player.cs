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
            _context.SignalRConnection.On<object>("ResultReport", OnResultReport);
            _context.SignalRConnection.On<object>("EndRound", OnEndRound);
            _context.SignalRConnection.On<object>("CreateRound", OnCreateRound);
            _context.SignalRConnection.On<object>("BetsClosed", OnBetsClosed);
            _context.SignalRConnection.On<object>("DoBetResult", DoBetResult);
            //_context.SignalRConnection.On<object>("UserBalance", UserBalance);
            //_context.SignalRConnection.On<object>("InitialDataResult", InitialDataResult);
            //_context.SignalRConnection.On<object>("RoundResult", OnRoundResult);
        }

        private void OnConnected(object data)
        {
            _context.IsInGame = true;
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

                _context.Balance = initialData.User.Balance;
                _context.MinBet = initialData.Game.MinBet;
                _context.MaxBet = initialData.Game.MaxBet;
                _context.MaxWin = initialData.Game.MaxWin;
                _context.CurrentStatus = initialData.Round.CurrentStatus;

                Console.WriteLine($"InitialDataResult -  {data}");
            }
        }

        private async Task OnResultReport(object data)
        {
            if (_context.SignalRConnection != null)
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

                _context.CurrentMultiplier = resultReportData.Odd;

                await Cashout(_context.RoundId);

                Console.WriteLine($"ResultReport - {data}");
            }
        }

        private async Task OnEndRound(object data)
        {
            if (_context.SignalRConnection != null)
            {
                Console.WriteLine($"EndRound - {data}");
                _context.IsCashedOut = false;
            }
        }

        private async Task OnCreateRound(object data)
        {
            if (_context.SignalRConnection != null)
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

                _context.RoundId = createRoundData.Id;

                await PlayGameRound(createRoundData.Id);

                Console.WriteLine($"CreateRound - {createRoundData.Id}");
            }
        }

        private async Task OnBetsClosed(object data)
        {
            if (_context.SignalRConnection != null)
            {
                Console.WriteLine($"BetsClosed - {data}");
            }
        }

        private async Task DoBet(BetRequestModel doBetRequestModel)
        {
            if (_context.SignalRConnection != null)
            {
                await Task.Delay(1000, _cancellationToken);

                await _context.SignalRConnection.SendAsync("DoBet", doBetRequestModel, _cancellationToken);
            }
        }

        private async Task PlayGameRound(Guid roundId)
        {
            if (_scenario.BetStrategy != null && await _scenario.BetStrategy.ShouldBetAsync(_context, _cancellationToken))
            {
                var betAmount = await _scenario.BetStrategy.GetBetAmountAsync(_context, _cancellationToken);

                var doBetModel = new BetRequestModel
                {
                    RoundId = roundId,
                    BetSection = BetSectionEnum.BetSectionLeft,
                    BetAmount = betAmount,
                    Name = "Crashaxy"
                };

                await DoBet(doBetModel);
            }
        }

        private async Task Cashout(Guid roundId)
        {
            if (roundId == Guid.Empty || _context.IsCashedOut)
            {
                return;
            }

            Console.WriteLine($"_context.IsInGame {_context.IsInGame}, _scenario.CashoutStrategy{_scenario.CashoutStrategy != null}");

            if (_context.IsInGame && _scenario.CashoutStrategy != null)
            {
                if (await _scenario.CashoutStrategy.ShouldCashoutAsync(_context, _cancellationToken))
                {
                    var cashOutRequest = new CashoutRequestModel
                    {
                        Odd = _context.CurrentMultiplier,
                        RoundId = roundId,
                        BetSection = BetSectionEnum.BetSectionLeft,
                        IsHalfCashout = false,
                        BetId = _context.BetId,
                        CashoutTime = DateTime.UtcNow
                    };

                    if (_context.SignalRConnection != null)
                    {
                        await _context.SignalRConnection.SendAsync("DoCashout", cashOutRequest, _cancellationToken);
                        Console.WriteLine($"Player {_context.PlayerId} cashed out, BetId = {cashOutRequest.BetId}");
                        _context.IsCashedOut = true;
                    }
                }
            }
        }

        private async Task DoBetResult(object data)
        {
            if (_context.SignalRConnection != null)
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

                _context.BetId = doBetResultData.BetId;

                Console.WriteLine($"DoBetResult - {data}");
            }
        }

        private async Task CleanupAsync()
        {
            if (_context.SignalRConnection != null)
            {
                await _context.SignalRConnection.DisposeAsync();
            }

            _playerPoolService.ReleasePlayer(_poolPlayer.PlayerId);
            _context.IsInGame = false;
            Console.WriteLine($"Player {_context.PlayerId} released back to pool");
        }
    }
}