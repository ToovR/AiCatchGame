using AiCatchGame.Bo;
using AiCatchGame.Bo.Exceptions;
using AiCatchGame.Web.Client.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace AiCatchGame.Web.Client.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly NavigationManager _navigation;
        private readonly INetClient _netClient;
        private HubConnection? _gameHubConnection = null;
        private ILocalStorageService _localStorage;

        public PlayerService(IAccessTokenProvider accessTokenProvider, ILocalStorageService localStorage, INetClient netClient, NavigationManager navigation)
        {
            _accessTokenProvider = accessTokenProvider;
            _navigation = navigation;
            _netClient = netClient;
            _localStorage = localStorage;
        }

        public async Task<ErrorCodes> AddPlayer(string pseudonym)
        {
            try
            {
                await _gameHubConnection.SendAsync("JoinGame", pseudonym);
                PlayerKeyInfo? playerId = await _netClient.PostAsync<string, PlayerKeyInfo>("/api/player", pseudonym);
                ArgumentNullException.ThrowIfNull(playerId);
                await _localStorage.SetItemAsync("PlayerPrivateId", playerId.PrivateId);
                await _localStorage.SetItemAsync("PlayerPublicId", playerId.PublicId);
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
            catch
            {
                return ErrorCodes.Undefined;
            }
        }

        public async Task<CharacterInfo> GetCharacterInfo(Guid setId)
        {
            Guid? playerPrivateId = await GetPlayerPrivateId();
            ArgumentNullException.ThrowIfNull(playerPrivateId);
            CharacterInfo? characterInfo = await _netClient.GetAsync<CharacterInfo>($"/character/{playerPrivateId}/{setId}/");
            ArgumentNullException.ThrowIfNull(characterInfo);
            return characterInfo;
        }

        public async Task<Guid?> GetGameId()
        {
            Guid? playerId = await GetPlayerPrivateId();
            if (playerId == null)
            {
                return null;
            }
            Guid? gameId = await _netClient.GetAsync<Guid>($"api/game/{playerId}/");
            return gameId;
        }

        public async Task NotifyReady()
        {
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            Guid? playerId = await GetPlayerPrivateId();
            ArgumentNullException.ThrowIfNull(playerId);
            await _gameHubConnection.SendAsync("SendPlayerReady", playerId.Value);
        }

        public async Task OnGameJoined()
        {
            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            _gameHubConnection.On<string, Guid>("GameJoined", async (string userId, Guid publicId) =>
            {
                await _localStorage.SetItemAsync("PlayerPrivateId", userId);
                await _localStorage.SetItemAsync("PlayerPublicId", publicId);
            });
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
            Guid? playerPublicId = await GetPlayerPublicId();
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
            Guid playerId = await _localStorage.GetItemAsync<Guid>("PlayerId");
            await _gameHubConnection.SendAsync("SendMessage", playerId, message);
        }

        public async Task Vote(Guid characterVotedId)
        {
            _gameHubConnection ??= InitializeGameHubConnection();
            ArgumentNullException.ThrowIfNull(_gameHubConnection);
            Guid playerId = await _localStorage.GetItemAsync<Guid>("PlayerId");
            await _gameHubConnection.SendAsync("Vote", playerId, characterVotedId);
        }

        private async Task<Guid?> GetPlayerPrivateId()
        {
            try
            {
                return await _localStorage.GetItemAsync<Guid>("PlayerPrivateId");
            }
            catch
            {
                return null;
            }
        }

        private async Task<Guid?> GetPlayerPublicId()
        {
            try
            {
                return await _localStorage.GetItemAsync<Guid>("PlayerPublicId");
            }
            catch
            {
                return null;
            }
        }

        private HubConnection InitializeGameHubConnection()
        {
            HubConnection gameHubConnection = new HubConnectionBuilder()
                  .WithUrl(_navigation.ToAbsoluteUri("/api/gamehub"), options =>
                   {
                       options.AccessTokenProvider = async () =>
                       {
                           var accessTokenResult = await _accessTokenProvider.RequestAccessToken();
                           accessTokenResult.TryGetToken(out var accessToken);
                           return accessToken.Value;
                       };
                   })
        .Build();
            return gameHubConnection;
        }
    }
}