using AiCatchGame.Bo;

namespace AiCatchGame.Web.Interfaces
{
    public interface IGameService
    {
        Task<Guid> AddPlayerToGame(string pseudonym, string privateId);

        Task<Guid> GetCharacterId(string playerId);

        Task<GameServer> GetGameById(Guid gameId);

        Task<GameServer> GetGameByPlayerId(string playerId);

        IEnumerable<GameServer> GetGames();

        Task<GameServer[]> GetGamesToStart();

        Task StartGame(Guid id);
    }
}