using AiCatchGame.Bo;
using AiCatchGame.Web.Interfaces;

namespace AiCatchGame.Web.Api
{
    public static class GameExtensions
    {
        public static RouteGroupBuilder MapGame(this RouteGroupBuilder group)
        {
            group.MapGet("id/{playerId}", async (string playerId, IGameService gameService) =>
            {
                GameServer? game = (await gameService.GetGameByPlayerId(playerId));
                if (game == null)
                {
                    return (Guid?)null;
                }
                return game.Id;
            });
            return group;
        }
    }
}