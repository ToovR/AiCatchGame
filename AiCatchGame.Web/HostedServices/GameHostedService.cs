using AiCatchGame.Bo;
using AiCatchGame.Web.Interfaces;
using AiCatchGame.Web.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.SignalR.Client;

namespace AiCatchGame.Web.HostedServices
{
    public class GameHostedService : IHostedService, IDisposable
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IServer _server;
        private List<AiPlayerClientService> _aiPlayerClients = [];
        private HubConnection? _gameHubConnection;
        private IServiceScopeFactory _serviceScopeFactory;
        private Timer? _timer = null;

        public GameHostedService(IServiceScopeFactory serviceScopeFactory, IHostApplicationLifetime hostApplicationLifetime, IServer server)
        {
            _server = server;
            _hostApplicationLifetime = hostApplicationLifetime;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                _gameHubConnection = InitializeGameHubConnection();
                _timer = new Timer(DoWork, null, TimeSpan.Zero,
                  TimeSpan.FromSeconds(5));
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IGameService gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
            await TreatGamesInLobby(gameService);
            await TreatGameSetChatToStart(gameService);
            await TreatGameSetChatToStop(gameService);
            await TreatGameSetVoteToStop(gameService);
        }

        private string GetDefaultAddres()
        {
            IServerAddressesFeature? serverAdressFeature = _server.Features.Get<IServerAddressesFeature>();

            ArgumentNullException.ThrowIfNull(serverAdressFeature);
            ICollection<string> addresses = serverAdressFeature.Addresses;
            return addresses.First();
        }

        private HubConnection InitializeGameHubConnection()
        {
            string baseUrl = GetDefaultAddres();
            HubConnection gameHubConnection = new HubConnectionBuilder()
                  .WithUrl($"{baseUrl}/gameHub").Build();
            return gameHubConnection;
        }

        private async Task TreatGameSetChatToStart(IGameService gameService)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            if (_gameHubConnection.State != HubConnectionState.Connected)
            {
                await _gameHubConnection.StartAsync();
            }
            GameServer[] games = await gameService.GetGamesToStartChat();
            foreach (GameServer game in games)
            {
                await _gameHubConnection.SendAsync("SetStartChat", game.Id);
            }
        }

        private async Task TreatGameSetChatToStop(IGameService gameService)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            if (_gameHubConnection.State != HubConnectionState.Connected)
            {
                await _gameHubConnection.StartAsync();
            }
            GameServer[] games = await gameService.GetGamesToStopChat();
            foreach (GameServer game in games)
            {
                await _gameHubConnection.SendAsync("SetEndChat", game.Id);
            }
        }

        private async Task TreatGameSetVoteToStop(IGameService gameService)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            if (_gameHubConnection.State != HubConnectionState.Connected)
            {
                await _gameHubConnection.StartAsync();
            }
            GameServer[] games = await gameService.GetGamesToStopVote();
            foreach (GameServer game in games)
            {
                await _gameHubConnection.SendAsync("SetEndVote", game.Id);
                game.GameSets.Last().Status = GameSetStatuses.End;
            }
        }

        private async Task TreatGamesInLobby(IGameService gameService)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            if (_gameHubConnection.State != HubConnectionState.Connected)
            {
                await _gameHubConnection.StartAsync();
            }
            GameServer[] games = await gameService.GetGamesToStart();
            foreach (GameServer game in games)
            {
                await _gameHubConnection.SendAsync("GameStart", game.Id);
                _aiPlayerClients.AddRange(gameService.InitializeAiPlayers(game));
            }
        }
    }
}