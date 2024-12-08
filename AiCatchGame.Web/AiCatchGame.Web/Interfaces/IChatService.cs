namespace AiCatchGame.Web.Interfaces
{
    public interface IChatService
    {
        Task PostMessage(string playerId, string message);
    }
}