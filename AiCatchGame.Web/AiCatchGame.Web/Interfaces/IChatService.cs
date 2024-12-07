namespace AiCatchGame.Web.Interfaces
{
    public interface IChatService
    {
        Task PostMessage(Guid playerId, string message);
    }
}
