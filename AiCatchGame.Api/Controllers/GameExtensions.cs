using AiCatchGame.Api.Interfaces;
using AiCatchGame.Bo;

namespace AiCatchGame.Api.Controllers
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