using AiCatchGame.Bo;
using AiCatchGame.Web.Client.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace AiCatchGame.Web.Client.Services
{
    public class HubClientService : IHubClientService
    {
        private HubConnection? _gameHubConnection = null;

        public HubClientService(NavigationManager navigation)
        {
            _gameHubConnection = new HubConnectionBuilder().WithUrl(navigation.ToAbsoluteUri("/gameHub")).Build();
        }

        public void OnGameJoined(Action<string, Guid> handler)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<string, Guid>("GameJoined", handler);
        }

        public async void OnGameStart(Action<GameClient> gameAction)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<GameClient>("GameStarted", (GameClient game) => gameAction(game));
        }

        public void OnNewPlayer(Action<string> onNewPlayer)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<string>("OnNewPlayer", (string pseudonym) => onNewPlayer(pseudonym));
        }

        public void OnReceivedMessage(Action<Guid, string> receivedMessageAction)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<Guid, string>("ReceivedMessage", (Guid characterId, string message) => receivedMessageAction(characterId, message));
        }

        public void OnSetStart(Action<GameSetClient> setStartAction)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<GameSetClient>("SetStarted", (GameSetClient gameSet) => setStartAction(gameSet));
        }

        public async Task SendPlayerReady(string playerId)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            if (_gameHubConnection.State != HubConnectionState.Connected)
            {
                await _gameHubConnection.StartAsync();
            }
            await _gameHubConnection.SendAsync("SendPlayerReady", playerId);
        }

        public async Task StartJoinGame(string pseudonym)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            await _gameHubConnection.StartAsync();
            await _gameHubConnection.SendAsync("JoinGame", pseudonym);
        }
    }
}