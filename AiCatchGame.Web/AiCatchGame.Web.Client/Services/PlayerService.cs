using AiCatchGame.Bo;
using AiCatchGame.Bo.Exceptions;
using AiCatchGame.Web.Client.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace AiCatchGame.Web.Client.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IStorageService _localStorage;
        private readonly NavigationManager _navigation;
        private readonly INetClient _netClient;
        private HubConnection? _gameHubConnection = null;

        public PlayerService(NavigationManager Navigation, IStorageService localStorage, INetClient netClient, NavigationManager navigation)
        {
            _gameHubConnection = new HubConnectionBuilder().WithUrl(Navigation.ToAbsoluteUri("/gameHub")).Build();
            _navigation = navigation;
            _netClient = netClient;
            _localStorage = localStorage;
        }

        public async Task<ErrorCodes> AddPlayer(string pseudonym)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_gameHubConnection);

                _gameHubConnection.On<string, Guid>("GameJoined", async (privateId, publicId) =>
                {
                    PlayerKeyInfo? playerId = new PlayerKeyInfo(privateId, publicId);
                    ArgumentNullException.ThrowIfNull(playerId);
                    await _localStorage.Set(LocalStorageKeys.PlayerPrivateId, playerId.PrivateId);
                    await _localStorage.Set(LocalStorageKeys.PlayerPublicId, playerId.PublicId);
                });

                await _gameHubConnection.StartAsync();
                await _gameHubConnection.SendAsync("JoinGame", pseudonym);

                string? playerId;

                // TODO ugly
                do
                {
                    playerId = await _localStorage.Get<string>(LocalStorageKeys.PlayerPrivateId);
                    await Task.Delay(500);
                } while (playerId == null);

                return ErrorCodes.None;
            }
            catch (AiCatchException ai)
            {
                if (ai.Code == ErrorCodes.AlreadyExists)
                {
                    return ErrorCodes.AlreadyExists;
                }
                return ErrorCodes.Undefined;
            }
            catch (Exception)
            {
                return ErrorCodes.Undefined;
            }
        }

        public async Task<CharacterInfo> GetCharacterInfo(Guid setId)
        {
            string? playerPrivateId = await _localStorage.Get<string>(LocalStorageKeys.PlayerPrivateId);
            ArgumentNullException.ThrowIfNull(playerPrivateId);
            CharacterInfo? characterInfo = await _netClient.GetAsync<CharacterInfo>($"/character/{playerPrivateId}/{setId}/");
            ArgumentNullException.ThrowIfNull(characterInfo);
            return characterInfo;
        }

        public async Task<Guid?> GetGameId()
        {
            string? playerId = await _localStorage.Get<string>(LocalStorageKeys.PlayerPrivateId);
            if (playerId == null)
            {
                return null;
            }
            Guid? gameId = await _netClient.GetAsync<Guid>($"api/game/id/{playerId}/");
            return gameId;
        }

        public async Task NotifyReady()
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            string? playerId = await _localStorage.Get<string>(LocalStorageKeys.PlayerPrivateId);
            ArgumentNullException.ThrowIfNull(playerId);
            await _gameHubConnection.SendAsync("SendPlayerReady", playerId);
        }

        public void OnGameStart(Action<GameClient> gameAction)
        {
            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<GameClient>("GameStart", (GameClient game) => gameAction(game));
        }

        public void OnReceivedMessage(Action<Guid, string> receivedMessageAction)
        {
            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<Guid, string>("ReceivedMessage", (Guid characterId, string message) => receivedMessageAction(characterId, message));
        }

        public async Task OnSetEnd(Func<GameSetResultInfo, PlayerGameSetResultInfo, Task> setEndAction)
        {
            Guid? playerPublicId = await _localStorage.Get<Guid>(LocalStorageKeys.PlayerPublicId);
            ArgumentNullException.ThrowIfNull(playerPublicId);

            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<GameSetResultInfo>("SetEnd", async (GameSetResultInfo gameSetResultInfo) =>
            {
                PlayerGameSetResultInfo playerInfo = gameSetResultInfo.Players.Single(p => p.PlayerId == playerPublicId);
                await setEndAction(gameSetResultInfo, playerInfo);
            });
        }

        public void OnSetSomeoneVoted(Action<SomeoneVotedInfo> someoneVotedAction)
        {
            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<SomeoneVotedInfo>("SetSomeoneVoted", (SomeoneVotedInfo someoneVotedInfo) => someoneVotedAction(someoneVotedInfo));
        }

        public void OnSetStart(Action<GameSetInfo> setStartAction)
        {
            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<GameSetInfo>("SetStart", (GameSetInfo gameSetInfo) => setStartAction(gameSetInfo));
        }

        public void OnSetStartChat(Action<GameSetChattingInfo> setStartChatAction)
        {
            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<GameSetChattingInfo>("SetStartChat", (GameSetChattingInfo gameSetChatingInfo) => setStartChatAction(gameSetChatingInfo));
        }

        public void OnSetStartVote(Action<GameSetVotingInfo> setStartVoteAction)
        {
            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<GameSetVotingInfo>("SetStartVote", (GameSetVotingInfo gameSetVotingInfo) => setStartVoteAction(gameSetVotingInfo));
        }

        public async Task SendMessage(string message)
        {
            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            string? playerId = await _localStorage.Get<string>(LocalStorageKeys.PlayerPrivateId);
            ArgumentNullException.ThrowIfNull(playerId);
            await _gameHubConnection.SendAsync("SendMessage", playerId, message);
        }

        public async Task Vote(Guid characterVotedId)
        {
            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            string? playerId = await _localStorage.Get<string>(LocalStorageKeys.PlayerPrivateId);
            await _gameHubConnection.SendAsync("Vote", playerId, characterVotedId);
        }

        private HubConnection InitializeGameHubConnection()
        {
            HubConnection gameHubConnection = new HubConnectionBuilder()
    .WithUrl(new Uri("/api/gamehub"))
    .WithAutomaticReconnect()
    .Build();
            return gameHubConnection;
        }
    }
}