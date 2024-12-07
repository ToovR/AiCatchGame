using AiCatchGame.Bo;
using AiCatchGame.Bo.Exceptions;
using AiCatchGame.Web.Interfaces;
using System.Globalization;

namespace AiCatchGame.Web.Bll
{
    public class GameService : IGameService
    {
        private List<GameServer> _games = [];

        private const int LOBBY_PLAYER_WAIING_SECONDS = 5;

        private const int PLAYERS_MAX = 10;

        private const int PLAYERS_MIN = 5;

        public async Task<Guid> AddPlayerToGame(string pseudonym, String privateId)
        {
            if (!_games.Any(g => g.Status == GameStatuses.InLobby))
            {
                GameServer newGame = await InitializeGame();
                _games.Add(newGame);
            }
            GameServer game = _games.First(g => g.Status == GameStatuses.InLobby);
            if (game.HumanPlayers.Any(hp => hp.Pseudonym.UnaccentLower() == pseudonym.UnaccentLower()))
            {
                throw new AiCatchException(ErrorCodes.AlreadyExists);
            }
            HumanPlayer humanPlayer = new(privateId, pseudonym);
            game.HumanPlayers.Add(humanPlayer);
            game.LastAddedPlayerTime = DateTime.Now;
            return humanPlayer.PublicId;
        }

        public Task<GameServer> GetGameById(Guid gameId)
        {
            return Task.FromResult(_games.Single(g => g.Id == gameId));
        }

        public Task<GameServer[]> GetGamesToStart()
        {
            GameServer[] gamesToStart = _games.Where(g => g.Status == GameStatuses.InLobby &&
                    ((g.HumanPlayers.Count >= PLAYERS_MIN && DateTime.Now > g.LastAddedPlayerTime.AddSeconds(LOBBY_PLAYER_WAIING_SECONDS)) ||
                    (g.HumanPlayers.Count >= PLAYERS_MAX))).ToArray();
            return Task.FromResult(gamesToStart);
        }

        public async Task<HumanPlayer> GetPlayerById(Guid playerId)
        {
            return _games.Select(g => g.HumanPlayers.First(p => p.Id == playerId)).First();
        }

        public Task StartGame(Guid gameId)
        {
            GameServer game = _games.Single(game => game.Id == gameId);
            game.Status = GameStatuses.Playing;
            game.GameStartedTime = DateTime.Now;
            return Task.CompletedTask;
        }

        private async Task<GameServer> InitializeGame()
        {
            GameServer newGame = new();
            AiPlayer aiPlayer = new();
            newGame.AiPlayers = [aiPlayer];
            return newGame;
        }

        public Task<GameServer> GetGameByPlayerId(Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> GetCharacterId(Guid playerId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GameServer> GetGames()
        {
            return _games;
        }
    }
}