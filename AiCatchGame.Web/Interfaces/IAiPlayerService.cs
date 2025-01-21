using AiCatchGame.Bo;
using AiCatchGame.Web.Services;

namespace AiCatchGame.Web.Interfaces
{
    public interface IAiPlayerService
    {
        Task OnPlayerSpeak(ChatMessage message);
    }
}