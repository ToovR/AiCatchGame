using AiCatchGame.Bo;
using AiCatchGame.Web.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AiCatchGame.Web.Api
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;

        public GameHub(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task GameStart(Guid gameId)
        {
            GameServer game = await _gameService.GetGameById(gameId);
            await _gameService.StartGame(game.Id);
            GameClient gameClient = new(GameStatuses.Playing);
            await GamePlayers(game).SendAsync("GameStarted", gameClient);
            // Initialize first set
            await InitializeSet(game);
        }

        public async Task JoinGame(string pseudonym)
        {
            string privateId = Context.ConnectionId;
            (Guid publicId, GameServer game) = await _gameService.AddPlayerToGame(pseudonym, privateId);
            await Clients.Caller.SendAsync("GameJoined", privateId, publicId);
            await GamePlayers(game).SendAsync("OnNewPlayer", pseudonym);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public Task OnReceivedMessage(Action<Guid, string> receivedMessageAction)
        {
            throw new NotImplementedException();
        }

        public Task OnSetSomeoneVoted(Action<SomeoneVotedInfo> someoneVotedAction)
        {
            throw new NotImplementedException();
        }

        public async Task OnSetStartChat(Action<GameSetChattingInfo> setStartChatAction)
        {
            await Clients.All.SendAsync("SetStartChat", (GameSetChattingInfo gameSetChatingInfo) => setStartChatAction(gameSetChatingInfo));
        }

        public async Task SendMessage(string playerId, string message)
        {
            GameServer game = await _gameService.GetGameByPlayerId(playerId);
            Guid characterId = await _gameService.GetCharacterId(playerId);

            await GamePlayers(game).SendAsync("ReceiveMessage", characterId, message);
        }

        public async Task SendPlayerReady(Guid player)
        {
            await Clients.All.SendAsync("SendPlayerReady", player);
        }

        /// <summary>
        /// At end of chat phase, send message to players to initialize vote
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public async Task SetEndChat(Guid gameId)
        {
            GameServer game = await _gameService.GetGameById(gameId);
            GameSetServer set = game.GameSets.Last();
            set.Status = GameSetStatuses.Voting;
            foreach (PlayerSetInfo playerInfo in set.PlayerSetInfoList)
            {
                GameSetVotingInfo gameSetVotingInfo = new(
                    set.PlayerSetInfoList.Where(c => c.CharacterId != playerInfo.CharacterId).Select(c => new CharacterInfo(c.CharacterId, c.CharacterName)).ToArray(),
                    game.Rules.VoteDuration
                );
                await Clients.Clients(playerInfo.PlayerPrivateId).SendAsync("SetStartVote", gameSetVotingInfo);
            }
            set.VotingStartTime = DateTime.Now;
        }

        public async Task SetEndVote(Guid gameId)
        {
            GameSetResultInfo resultInfo = await _gameService.GetSetResultInfo(gameId);
            GameServer game = await _gameService.GetGameById(gameId);
            await GamePlayers(game).SendAsync("ShowScore", resultInfo);
        }

        public async Task Vote(string playerId, Guid characterVotedId)
        {
            DateTime timestampVote = DateTime.Now;
            GameServer game = await _gameService.GetGameByPlayerId(playerId);
            GameSetServer set = game.GameSets.Last();
            ArgumentNullException.ThrowIfNull(set.VotingStartTime);
            double timeReaction = (timestampVote - set.VotingStartTime.Value).TotalSeconds;
            set.Votes.Add(new VoteInfo(playerId, characterVotedId, timeReaction));
            if (set.Votes.Count >= game.HumanPlayers.Count)
            {
                await SetEndVote(game.Id);
            }
        }

        private IClientProxy GamePlayers(GameServer game)
        {
            return Clients.Clients(game.PlayerPrivateIds);
        }

        private async Task InitializeSet(GameServer game)
        {
            GameSetServer setinfo = await _gameService.InitializeSetInfo(game.Id);
            CharacterInfo[] characterList = setinfo.PlayerSetInfoList.Select(p => new CharacterInfo(p.CharacterId, p.CharacterName)).ToArray();
            foreach (PlayerSetInfo playerInfo in setinfo.PlayerSetInfoList)
            {
                GameSetClient gameSetClient = new(playerInfo, characterList, setinfo.RoundNumber, setinfo.Status);
                await Clients.Clients(playerInfo.PlayerPrivateId).SendAsync("SetStarted", gameSetClient);
            }
        }
    }
}