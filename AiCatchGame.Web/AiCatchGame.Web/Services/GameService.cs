using AiCatchGame.Bo;
using AiCatchGame.Bo.Exceptions;
using AiCatchGame.Web.Interfaces;

namespace AiCatchGame.Web.Services
{
    public class GameService : IGameService
    {
        private const int LOBBY_PLAYER_WAIING_SECONDS = 5;
        private const int PLAYERS_MAX = 10;
        private const int PLAYERS_MIN = 2;
        private static readonly List<GameServer> _games = [];

        public async Task<Tuple<Guid, GameServer>> AddPlayerToGame(string pseudonym, String privateId)
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
            return new Tuple<Guid, GameServer>(humanPlayer.PublicId, game);
        }

        public async Task<Guid> GetCharacterId(string playerId)
        {
            GameServer game = await GetGameByPlayerId(playerId);
            PlayerSetInfo? playerSetInfo = game.GameSets.Last().PlayerSetInfoList.FirstOrDefault(ps => ps.PlayerPrivateId == playerId);
            ArgumentNullException.ThrowIfNull(playerSetInfo);
            return playerSetInfo.CharacterId;
        }

        public Task<GameServer> GetGameById(Guid gameId)
        {
            return Task.FromResult(_games.Single(g => g.Id == gameId));
        }

        public Task<GameServer> GetGameByPlayerId(string playerId)
        {
            GameServer game = _games.First(g => g.PlayerPrivateIds.Any(p => playerId == p));
            return Task.FromResult(game);
        }

        public IEnumerable<GameServer> GetGames()
        {
            return _games;
        }

        public Task<GameServer[]> GetGamesToStart()
        {
            // See if there is enough players in the lobby to start a game
            // Minimum players : PLAYERS_MIN(4), wait LOBBY_PLAYER_WAIING_SECONDS(5) seconds and start
            // at PLAYERS_MAX(10), start immediately
            GameServer[] gamesToStart = _games.Where(g => g.Status == GameStatuses.InLobby &&
                    ((g.HumanPlayers.Count >= PLAYERS_MIN && DateTime.Now > g.LastAddedPlayerTime.AddSeconds(LOBBY_PLAYER_WAIING_SECONDS)) ||
                    (g.HumanPlayers.Count >= PLAYERS_MAX))).ToArray();
            return Task.FromResult(gamesToStart);
        }

        public Task<HumanPlayer> GetPlayerById(string playerId)
        {
            return Task.FromResult(_games.Select(g => g.HumanPlayers.First(p => p.PrivateId == playerId)).First());
        }

        public async Task<GameSetServer> InitializeSetInfo(Guid gameId)
        {
            GameServer game = await GetGameById(gameId);
            GameSetService gameSetService = new(this);
            GameSetServer setInfo = await gameSetService.InitializeSet(game);
            return setInfo;
            throw new NotImplementedException();
        }

        public Task StartGame(Guid gameId)
        {
            GameServer game = _games.Single(game => game.Id == gameId);
            game.Status = GameStatuses.Playing;
            game.GameStartedTime = DateTime.Now;
            return Task.CompletedTask;
        }

        private Task<GameServer> InitializeGame()
        {
            GameServer newGame = new();
            AiPlayer aiPlayer = new("Ai1");
            newGame.AiPlayers = [aiPlayer];
            return Task.FromResult(newGame);
        }
    }
}