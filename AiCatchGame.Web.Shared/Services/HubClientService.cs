﻿using AiCatchGame.Bo;
using AiCatchGame.Web.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace AiCatchGame.Web.Shared.Services
{
    public class HubClientService : IHubClientService
    {
        private HubConnection? _gameHubConnection = null;

        public HubClientService()
        {
        }

        public void Initialize(string url)
        {
            _gameHubConnection = new HubConnectionBuilder().WithUrl(url).Build();
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

        public Task OnReceivedMessage(Func<CTAChatMessage, Task> receivedMessageAction)
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
            _gameHubConnection.On<GameSetClient>("SetStarted", async (gsc) => await setStartAction(gsc));
            return Task.CompletedTask;
        }

        public Task OnSetStartChat(Func<GameSetChattingInfo, Task> setStartChatAction)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<GameSetChattingInfo>("OnSetStartChat", async (gsci) => await setStartChatAction(gsci));
            return Task.CompletedTask;
        }

        public Task OnSetStartVote(Func<GameSetVotingInfo, Task> setStartVoteAction)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On("SetStartVote", setStartVoteAction);
            return Task.CompletedTask;
        }

        public async Task SendMessage(string playerId, string message)
        {
            await (await SafeSend()).SendAsync("SendMessage", playerId, message);
        }

        public async Task SendPlayerReady(string playerId)
        {
            await (await SafeSend()).SendAsync("SendPlayerReady", playerId);
        }

        public async Task StartJoinGame(string pseudonym)
        {
            await (await SafeSend()).SendAsync("JoinGame", pseudonym);
        }

        public async Task Vote(string playerId, Guid characterVotedId)
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            await (await SafeSend()).SendAsync("Vote", playerId, characterVotedId);
        }

        private async Task<HubConnection> SafeSend()
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            if (_gameHubConnection.State != HubConnectionState.Connected)
            {
                await _gameHubConnection.StartAsync();
                Console.WriteLine($"connection id :{_gameHubConnection.ConnectionId}");
            }
            return _gameHubConnection;
        }
    }
}