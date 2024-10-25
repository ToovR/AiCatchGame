using AiCatchGame.Bo;
using AiCatchGame.Web.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AiCatchGame.Web
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;

        public GameHub(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task JoinGame(string pseudonym)
        {
            string userId = Context.User.Identity.Name;
            Guid publicId = await _gameService.AddPlayerToGame(pseudonym, userId);
            await Clients.Caller.SendAsync("GameJoined", userId, publicId);
        }

        public async Task OnReceivedMessage(Action<Guid, string> receivedMessageAction)
        {
        }

        public async Task OnSetEnd(Action<GameSetResultInfo> setEndAction)
        {
        }

        public async Task OnSetSomeoneVoted(Action<SomeoneVotedInfo> someoneVotedAction)
        {
        }

        public async Task OnSetStart(Action<GameSetInfo> setStartAction)
        {
        }

        public async Task OnSetStartChat(Action<GameSetChatingInfo> setStartChatAction)
        {
            _gameHubConnection.On<GameSetChatingInfo>("SetStartChat", (GameSetChatingInfo gameSetChatingInfo) => setStartChatAction(gameSetChatingInfo));
        }

        public async Task OnSetStartVote(Action<GameSetVotingInfo> setStartVoteAction)
        {
            _gameHubConnection.On<GameSetVotingInfo>("SetStartVote", (GameSetVotingInfo gameSetVotingInfo) => setStartVoteAction(gameSetVotingInfo));
        }

        public async Task SendMessage(Guid playerId, string message)
        {
            GameServer game = await _gameService.GetGameByPlayerId(playerId);
            Guid characterId = await _gameService.GetCharacterId(playerId);

            await Clients.Users(game.HumanPlayers.Select(p => p.Id.ToString())).All.SendAsync("ReceiveMessage", characterId, message);
        }

        public async Task SendPlayerReady(Guid player)
        {
            _gameService.
        }

        public async Task StartGame(Guid gameId)
        {
            GameServer game = await _gameService.GetGameById(gameId);
            await _gameService.StartGame(game.Id);
            GameClient gameClient = new GameClient(GameStatuses.Playing);
            await Clients.Users(game.PlayerIds).SendAsync("StartGame", gameClient);
        }

        public async Task StartGame(GGame game)
        {
            await Clients.All.SendAsync("GameStart", game);
        }
    }
}