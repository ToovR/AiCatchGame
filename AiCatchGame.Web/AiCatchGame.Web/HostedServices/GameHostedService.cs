using AiCatchGame.Bo;
using AiCatchGame.Web.Interfaces;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.SignalR.Client;

namespace AiCatchGame.Web.HostedServices
{
    public class GameHostedService : IHostedService, IDisposable
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IServer _server;
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
                await _gameHubConnection.SendAsync("StartGame", game.Id);
            }
        }
    }
}