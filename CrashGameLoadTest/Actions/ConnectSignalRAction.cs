using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace CrashGameLoadTest.Actions
{
    public class ConnectSignalRAction(string hubUrl) : IPlayerAction
    {
        private readonly string _hubUrl = hubUrl;

        public async Task ExecuteAsync(PlayerContext context, CancellationToken token)
        {
            context.SignalRConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl, opts => { opts.AccessTokenProvider = () => Task.FromResult(context.JwtToken)!; })
                .WithAutomaticReconnect()
                .Build();

            await context.SignalRConnection.StartAsync(token);
        }
    }
}