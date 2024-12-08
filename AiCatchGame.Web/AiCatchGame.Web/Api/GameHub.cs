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

        public async Task JoinGame(string pseudonym)
        {
            string privateId = Context.ConnectionId;
            Guid publicId = await _gameService.AddPlayerToGame(pseudonym, privateId);
            await Clients.Caller.SendAsync("GameJoined", privateId, publicId);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public Task OnReceivedMessage(Action<Guid, string> receivedMessageAction)
        {
            throw new NotImplementedException();
        }

        public Task OnSetEnd(Action<GameSetResultInfo> setEndAction)
        {
            throw new NotImplementedException();
        }

        public Task OnSetSomeoneVoted(Action<SomeoneVotedInfo> someoneVotedAction)
        {
            throw new NotImplementedException();
        }

        public Task OnSetStart(Action<GameSetInfo> setStartAction)
        {
            throw new NotImplementedException();
        }

        public async Task OnSetStartChat(Action<GameSetChattingInfo> setStartChatAction)
        {
            await Clients.All.SendAsync("SetStartChat", (GameSetChattingInfo gameSetChatingInfo) => setStartChatAction(gameSetChatingInfo));
        }

        public async Task OnSetStartVote(Action<GameSetVotingInfo> setStartVoteAction)
        {
            await Clients.All.SendAsync("SetStartVote", (GameSetVotingInfo gameSetVotingInfo) => setStartVoteAction(gameSetVotingInfo));
        }

        public async Task SendMessage(string playerId, string message)
        {
            GameServer game = await _gameService.GetGameByPlayerId(playerId);
            Guid characterId = await _gameService.GetCharacterId(playerId);

            await Clients.Users(game.HumanPlayers.Select(p => p.PrivateId)).SendAsync("ReceiveMessage", characterId, message);
        }

        public async Task SendPlayerReady(Guid player)
        {
            await Clients.All.SendAsync("SendPlayerReady", player);
        }

        public async Task StartGame(Guid gameId)
        {
            GameServer game = await _gameService.GetGameById(gameId);
            await _gameService.StartGame(game.Id);
            GameClient gameClient = new GameClient(GameStatuses.Playing);
            await Clients.Users(game.PlayerIds).SendAsync("StartGame", gameClient);
        }

        public Task Vote(string playerId, Guid characterId)
        {
            throw new NotImplementedException();
        }
    }
}