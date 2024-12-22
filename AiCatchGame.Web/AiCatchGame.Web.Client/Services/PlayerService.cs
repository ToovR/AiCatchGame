using AiCatchGame.Bo;
using AiCatchGame.Bo.Exceptions;
using AiCatchGame.Web.Client.Interfaces;

namespace AiCatchGame.Web.Client.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IHubClientService _hubClientService;
        private readonly IStorageService _localStorage;
        private readonly INetClient _netClient;

        public PlayerService(IStorageService localStorage, INetClient netClient, IHubClientService hubClientService)
        {
            _hubClientService = hubClientService;
            _netClient = netClient;
            _localStorage = localStorage;
        }

        public async Task<ErrorCodes> AddPlayer(string pseudonym)
        {
            try
            {
                await _localStorage.Remove(LocalStorageKeys.PlayerPrivateId);
                await _localStorage.Remove(LocalStorageKeys.PlayerPublicId);
                ArgumentNullException.ThrowIfNull(_hubClientService);

                await _hubClientService.OnGameJoined(async (privateId, publicId) =>
                {
                    PlayerKeyInfo? playerId = new PlayerKeyInfo(privateId, publicId);
                    ArgumentNullException.ThrowIfNull(playerId);
                    await _localStorage.Set(LocalStorageKeys.PlayerPrivateId, playerId.PrivateId);
                    await _localStorage.Set(LocalStorageKeys.PlayerPublicId, playerId.PublicId);
                });

                await _hubClientService.StartJoinGame(pseudonym);

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

        /// <summary>
        /// TODO check if used
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
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
            ArgumentNullException.ThrowIfNull(_hubClientService);
            string? playerId = await _localStorage.Get<string>(LocalStorageKeys.PlayerPrivateId);
            ArgumentNullException.ThrowIfNull(playerId);
            await _hubClientService.SendPlayerReady(playerId);
        }

        public Task OnGameStart(Func<GameClient, Task> gameAction)
        {
            ArgumentNullException.ThrowIfNull(_hubClientService);
            _hubClientService.OnGameStart(gameAction);
            return Task.CompletedTask;
        }

        public async Task OnNewPlayer(Func<string, Task> onNewPlayer)
        {
            ArgumentNullException.ThrowIfNull(_hubClientService);
            await _hubClientService.OnNewPlayer(onNewPlayer);
        }

        public async Task OnReceivedMessage(Func<Guid, string, Task> receivedMessageAction)
        {
            ArgumentNullException.ThrowIfNull(_hubClientService);
            await _hubClientService.OnReceivedMessage(receivedMessageAction);
        }

        public Task OnSetSomeoneVoted(Func<SomeoneVotedInfo, Task> someoneVotedAction)
        {
            //_gameHubConnection ??= InitializeGameHubConnection();
            //ArgumentNullException.ThrowIfNull(_gameHubConnection);
            //_gameHubConnection.On<SomeoneVotedInfo>("SetSomeoneVoted", (SomeoneVotedInfo someoneVotedInfo) => someoneVotedAction(someoneVotedInfo));
            return Task.CompletedTask;
        }

        public async Task OnSetStart(Func<GameSetClient, Task> setStartAction)
        {
            ArgumentNullException.ThrowIfNull(_hubClientService);
            await _hubClientService.OnSetStart(setStartAction);
            //_gameHubConnection ??= InitializeGameHubConnection();
            //ArgumentNullException.ThrowIfNull(_gameHubConnection);
            //_gameHubConnection.On<GameSetInfo>("SetStart", (GameSetInfo gameSetInfo) => setStartAction(gameSetInfo));
        }

        public Task OnSetStartChat(Func<GameSetChattingInfo, Task> setStartChatAction)
        {
            //_gameHubConnection ??= InitializeGameHubConnection();
            //ArgumentNullException.ThrowIfNull(_gameHubConnection);
            //_gameHubConnection.On<GameSetChattingInfo>("SetStartChat", (GameSetChattingInfo gameSetChatingInfo) => setStartChatAction(gameSetChatingInfo));
            return Task.CompletedTask;
        }

        public async Task OnSetStartVote(Func<GameSetVotingInfo, Task> setStartVoteAction)
        {
            ArgumentNullException.ThrowIfNull(_hubClientService);
            await _hubClientService.OnSetStartVote(setStartVoteAction);
        }

        public async Task OnShowScore(Func<GameSetResultInfo, Task> setShowScoreAction)
        {
            ArgumentNullException.ThrowIfNull(_hubClientService);
            await _hubClientService.OnSetShowScore(setShowScoreAction);

            //Guid? playerPublicId = await _localStorage.Get<Guid>(LocalStorageKeys.PlayerPublicId);
            //ArgumentNullException.ThrowIfNull(playerPublicId);

            //_gameHubConnection ??= InitializeGameHubConnection();
            //ArgumentNullException.ThrowIfNull(_gameHubConnection);
            //_gameHubConnection.On<GameSetResultInfo>("SetEnd", async (GameSetResultInfo gameSetResultInfo) =>
            //{
            //    PlayerGameSetResultInfo playerInfo = gameSetResultInfo.Players.Single(p => p.PlayerId == playerPublicId);
            //    await setEndAction(gameSetResultInfo, playerInfo);
            //});
        }

        public Task SendMessage(string message)
        {
            //_gameHubConnection ??= InitializeGameHubConnection();
            //ArgumentNullException.ThrowIfNull(_gameHubConnection);
            //string? playerId = await _localStorage.Get<string>(LocalStorageKeys.PlayerPrivateId);
            //ArgumentNullException.ThrowIfNull(playerId);
            //await _gameHubConnection.SendAsync("SendMessage", playerId, message);
            return Task.CompletedTask;
        }

        public async Task Vote(Guid characterVotedId)
        {
            ArgumentNullException.ThrowIfNull(_hubClientService);
            string? playerId = await _localStorage.Get<string>(LocalStorageKeys.PlayerPrivateId);
            ArgumentNullException.ThrowIfNull(playerId);
            await _hubClientService.Vote(playerId, characterVotedId);
        }

        //    private HubConnection InitializeGameHubConnection()
        //    {
        //        HubConnection gameHubConnection = new HubConnectionBuilder()
        //.WithUrl(new Uri("/api/gamehub"))
        //.WithAutomaticReconnect()
        //.Build();
        //        return gameHubConnection;
        //    }
    }
}