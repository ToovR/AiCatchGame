namespace AiCatchGame.Api.Interfaces
{
    public interface IChatService
    {
        Task PostMessage(string playerId, string message);
    }
}