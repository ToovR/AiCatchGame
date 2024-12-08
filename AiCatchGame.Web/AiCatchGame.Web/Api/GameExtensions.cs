using AiCatchGame.Web.Interfaces;

namespace AiCatchGame.Web.Api
{
    public static class GameExtensions
    {
        public static RouteGroupBuilder MapGame(this RouteGroupBuilder group)
        {
            group.MapGet("id/{playerId}", async (string playerId, IGameService gameService) => (await gameService.GetGameByPlayerId(playerId)).Id);
            return group;
        }
    }
}