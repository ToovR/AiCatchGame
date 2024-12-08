using AiCatchGame.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AiCatchGame.Web.Api
{
    public static class PlayerExtensions
    {
        public static RouteGroupBuilder MapPlayer(this RouteGroupBuilder group)
        {
            //group.MapPost("", async ([FromBody] string pseudonym, IGameService gameService) => await gameService.AddPlayerToGame(pseudonym));
            return group;
        }
    }
}