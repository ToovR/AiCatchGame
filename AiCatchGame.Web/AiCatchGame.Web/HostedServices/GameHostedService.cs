using AiCatchGame.Bo;
using AiCatchGame.Web.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace AiCatchGame.Web.HostedServices
{
    public class GameHostedService : IHostedService, IDisposable
    {
        private HubConnection _gameHubConnection;
        private IServiceScopeFactory _serviceScopeFactory;
        private Timer? _timer = null;

        public GameHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            _gameHubConnection = InitializeGameHubConnection();
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IGameService gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
            await TreatGamesInLobby(gameService);
        }

        private HubConnection InitializeGameHubConnection()
        {
            HubConnection gameHubConnection = new HubConnectionBuilder()
                  .WithUrl("/api/gamehub").Build();
            return gameHubConnection;
        }

        private async Task TreatGamesInLobby(IGameService gameService)
        {
            GameServer[] games = await gameService.GetGamesToStart();
            foreach (GameServer game in games)
            {
                await _gameHubConnection.SendAsync("StartGame", game.Id);
                gameService.
            }
        }
    }
}