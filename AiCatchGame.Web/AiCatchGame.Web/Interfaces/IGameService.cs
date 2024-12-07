using AiCatchGame.Bo;

namespace AiCatchGame.Web.Interfaces
{
    public interface IGameService
    {
        Task<Guid> AddPlayerToGame(string pseudonym, string privateId);

        Task<GameServer> GetGameById(Guid gameId);

        Task<GameServer> GetGameByPlayerId(Guid playerId);
        Task<GameServer[]> GetGamesToStart();
        Task<Guid> GetCharacterId(Guid playerId);
        Task StartGame(Guid id);
        IEnumerable<GameServer> GetGames();
    }
}