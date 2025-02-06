using AiCatchGame.Bo;

namespace AiCatchGame.Api.Interfaces
{
    public interface IAiPlayerService
    {
        Task InitializeAi();

        IEnumerable<AiChatMessage> ManageResponse();

        Task OnPlayerSpeak(CTAChatMessage message, AiPlayer aiPlayer);
    }
}