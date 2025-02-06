namespace AiCatchGame.Api.Interfaces
{
    public interface IAiLllmClient
    {
        Task<string> GenerateText(string prompt);

        Task Initialize();

        void SetSystemMessage(string defaultSystemMessage);

        //  Task Initialize(string defaultSystemMessage);
    }
}