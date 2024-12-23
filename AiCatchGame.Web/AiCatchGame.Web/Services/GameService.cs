using AiCatchGame.Bo;
using AiCatchGame.Bo.Exceptions;
using AiCatchGame.Web.Interfaces;

namespace AiCatchGame.Web.Services
{
    public class GameService : IGameService
    {
        private const int LOBBY_PLAYER_WAIING_SECONDS = 5;
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

        public async Task<Guid?> GetCharacterId(string playerId)
        {
            GameServer? game = await GetGameByPlayerId(playerId);
            if (game == null)
            {
                return null;
            }
            PlayerSetInfo? playerSetInfo = game.GameSets.Last().PlayerSetInfoList.FirstOrDefault(ps => ps.PlayerPrivateId == playerId);
            ArgumentNullException.ThrowIfNull(playerSetInfo);
            return playerSetInfo.CharacterId;
        }

        public Task<GameServer> GetGameById(Guid gameId)
        {
            return Task.FromResult(_games.Single(g => g.Id == gameId));
        }

        public Task<GameServer?> GetGameByPlayerId(string playerId)
        {
            GameServer? game = _games.FirstOrDefault(g => g.PlayerPrivateIds.Any(p => playerId == p));
            return Task.FromResult(game);
        }

        public IEnumerable<GameServer> GetGames()
        {
            return _games;
        }

        public Task<GameServer[]> GetGamesToStart()
        {
            // See if there is enough players in the lobby to start a game
            // Minimum players : PlayerMin(4), wait LOBBY_PLAYER_WAIING_SECONDS(5) seconds and start
            // at PlayerMax(10), start immediately
            GameServer[] gamesToStart = _games.Where(g => g.Status == GameStatuses.InLobby &&
                    ((g.HumanPlayers.Count >= g.Rules.PlayerMin && DateTime.Now > g.LastAddedPlayerTime.AddSeconds(LOBBY_PLAYER_WAIING_SECONDS)) ||
                    (g.HumanPlayers.Count >= g.Rules.PlayerMax))).ToArray();
            return Task.FromResult(gamesToStart);
        }

        public Task<GameServer[]> GetGamesToStopChat()
        {
            GameServer[] gamesToStopChat = _games.Where(g => g.Status == GameStatuses.Playing &&
                                g.GameSets.Any() && g.GameSets.Last().Status == GameSetStatuses.Chatting && g.GameSets.Last().ChattingStartTime.HasValue &&
                                DateTime.Now > g.GameSets.Last().ChattingStartTime!.Value.AddSeconds(g.Rules.ChatDuration)).ToArray();
            return Task.FromResult(gamesToStopChat);
        }

        public Task<GameServer[]> GetGamesToStopVote()
        {
            GameServer[] gamesToStopVote = _games.Where(g => g.Status == GameStatuses.Playing &&
                              g.GameSets.Any() && g.GameSets.Last().Status == GameSetStatuses.Voting && g.GameSets.Last().VotingStartTime.HasValue &&
                              DateTime.Now > g.GameSets.Last().VotingStartTime!.Value.AddSeconds(g.Rules.VoteDuration)).ToArray();
            return Task.FromResult(gamesToStopVote);
        }

        public Task<HumanPlayer> GetPlayerById(string playerId)
        {
            return Task.FromResult(_games.Select(g => g.HumanPlayers.First(p => p.PrivateId == playerId)).First());
        }

        public async Task<GameSetResultInfo> GetSetResultInfo(Guid gameId)
        {
            GameServer game = await GetGameById(gameId);
            GameSetServer set = game.GameSets.Last();
            GameSetResultInfo result = new()
            {
                AiInfoList = set.PlayerSetInfoList.Where(p => p.IsAi).Select(p => new AiGameSetResultInfo(new(p.CharacterId, p.CharacterName))).ToArray(),
                HumanPlayers = set.PlayerSetInfoList.Where(p => !p.IsAi).Select(p => GetHumanPlayerResultInfo(p, game, set)).ToArray(),
            };
            return result;
        }

        public async Task<GameSetServer> InitializeSetInfo(Guid gameId)
        {
            GameServer game = await GetGameById(gameId);
            GameSetService gameSetService = new(this);
            GameSetServer setInfo = await gameSetService.InitializeSet(game);
            return setInfo;
        }

        public Task StartGame(Guid gameId)
        {
            GameServer game = _games.Single(game => game.Id == gameId);
            game.Status = GameStatuses.Playing;
            game.GameStartedTime = DateTime.Now;
            return Task.CompletedTask;
        }

        private HumanPlayerGameSetResultInfo GetHumanPlayerResultInfo(PlayerSetInfo playerSetInfo, GameServer game, GameSetServer set)
        {
            HumanPlayer playerGameInfo = game.HumanPlayers.Single(hp => hp.PrivateId == playerSetInfo.PlayerPrivateId);
            bool hasFoundAi = false;
            double? voteReaction = null;
            if (set.Votes.Any(v => v.VoterId == playerSetInfo.PlayerPrivateId))
            {
                VoteInfo playerVote = set.Votes.Single(v => v.VoterId == playerSetInfo.PlayerPrivateId);
                hasFoundAi = set.PlayerSetInfoList.Single(p => p.CharacterId == playerVote.VotedId).IsAi;
                voteReaction = playerVote.TimeReaction;
            }

            Guid[] playerIdsVotedFor = set.Votes.Where(v => v.VotedId == playerSetInfo.CharacterId).Select(v => game.HumanPlayers.Single(hp => hp.PrivateId == v.VoterId).PublicId).ToArray();

            int scoreVote = 0;
            if (voteReaction != null)
            {
                scoreVote = (int)((game.Rules.ScoreVoteMax / game.Rules.VoteDuration) * (game.Rules.VoteDuration - voteReaction));
                if (!hasFoundAi)
                {
                    scoreVote = scoreVote * -1;
                }
            }

            int scoreVotedAsAi = playerIdsVotedFor.Length * game.Rules.ScoreVotedFor;

            int previousScoreTotal = 0;
            if (game.GameSets.Count > 1)
            {
                previousScoreTotal = playerGameInfo.SetResults[game.GameSets.Count - 2].ScoreTotal;
            }

            HumanPlayerGameSetResultInfo humanPlayerGameSetResultInfo = new HumanPlayerGameSetResultInfo
            {
                CharacterId = playerSetInfo.CharacterId,
                HasFoundAi = hasFoundAi,
                Pseudonym = playerGameInfo.Pseudonym,
                PlayerId = playerGameInfo.PublicId,
                PlayerIdsVotedFor = playerIdsVotedFor,
                Status = PlayerStatuses.Playing,
                VoteReaction = voteReaction,
                ScoreVote = scoreVote,
                ScoreVotedAsAi = scoreVotedAsAi,
                ScoreSet = scoreVote + scoreVotedAsAi,
                ScoreTotal = previousScoreTotal + scoreVote + scoreVotedAsAi,
            };
            playerGameInfo.SetResults.Insert(game.GameSets.Count - 1, humanPlayerGameSetResultInfo);
            return humanPlayerGameSetResultInfo;
        }

        private Task<GameServer> InitializeGame()
        {
            GameServer newGame = new(GameRuleInfo.GetDefault());
            AiPlayer aiPlayer = new("Ai1");
            newGame.AiPlayers = [aiPlayer];
            return Task.FromResult(newGame);
        }
    }
}