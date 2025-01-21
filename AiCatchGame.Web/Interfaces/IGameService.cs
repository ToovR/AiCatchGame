using AiCatchGame.Bo;
using AiCatchGame.Web.Services;

namespace AiCatchGame.Web.Interfaces
{
    public interface IGameService
    {
        Task<Tuple<Guid, GameServer>> AddPlayerToGame(string pseudonym, String privateId);

        Task<Guid?> GetCharacterId(string playerId);

        Task<GameServer> GetGameById(Guid gameId);

        Task<GameServer?> GetGameByPlayerId(string playerId);

        IEnumerable<GameServer> GetGames();

        Task<GameServer[]> GetGamesToStart();

        Task<GameServer[]> GetGamesToStartChat();

        Task<GameServer[]> GetGamesToStopChat();

        Task<GameServer[]> GetGamesToStopVote();

        Task<GameSetResultInfo> GetSetResultInfo(Guid gameId);

        IEnumerable<AiPlayerClientService> InitializeAiPlayers(GameServer game);

        Task<GameSetServer> InitializeSetInfo(Guid gameId);

        Task StartGame(Guid gameId);
    }
}