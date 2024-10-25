using AiCatchGame.Bo;

namespace AiCatchGame.Web.Interfaces
{
    public interface IGameService
    {
        Task<Guid> AddPlayerToGame(string pseudonym, string userId);

        Task<GameServer> GetGameById(Guid gameId);

        Task<GameServer[]> GetGamesToStart();

        Task StartGame(Guid id);
    }
}