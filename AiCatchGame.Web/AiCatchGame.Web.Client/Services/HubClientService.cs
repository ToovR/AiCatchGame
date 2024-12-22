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

        public Task OnGameJoined(Func<string, Guid, Task> handler)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On("GameJoined", handler);
            return Task.CompletedTask;
        }

        public Task OnGameStart(Func<GameClient, Task> gameAction)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On("GameStarted", gameAction);
            return Task.CompletedTask;
        }

        public Task OnNewPlayer(Func<string, Task> onNewPlayer)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On("OnNewPlayer", onNewPlayer);
            return Task.CompletedTask;
        }

        public Task OnReceivedMessage(Func<Guid, string, Task> receivedMessageAction)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On("ReceivedMessage", receivedMessageAction);
            return Task.CompletedTask;
        }

        public Task OnSetShowScore(Func<GameSetResultInfo, Task> setShowScoreAction)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On("ShowScore", setShowScoreAction);
            return Task.CompletedTask;
        }

        public Task OnSetStart(Func<GameSetClient, Task> setStartAction)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On("SetStarted", setStartAction);
            return Task.CompletedTask;
        }

        public Task OnSetStartVote(Func<GameSetVotingInfo, Task> setStartVoteAction)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On("SetStartVote", setStartVoteAction);
            return Task.CompletedTask;
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

        public async Task Vote(string playerId, Guid characterVotedId)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            if (_gameHubConnection.State != HubConnectionState.Connected)
            {
                await _gameHubConnection.StartAsync();
            }
            await _gameHubConnection.SendAsync("Vote", playerId, characterVotedId);
        }
    }
}