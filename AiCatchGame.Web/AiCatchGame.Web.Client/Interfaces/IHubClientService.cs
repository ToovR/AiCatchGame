using AiCatchGame.Bo;

namespace AiCatchGame.Web.Client.Interfaces
{
    public interface IHubClientService
    {
        void OnGameJoined(Action<string, Guid> handler);

        void OnGameStart(Action<GameClient> gameAction);

        void OnNewPlayer(Action<string> onNewPlayer);

        void OnReceivedMessage(Action<Guid, string> receivedMessageAction);

        void OnSetStart(Action<GameSetClient> setStartAction);

        Task SendPlayerReady(string playerId);

        Task StartJoinGame(string pseudonym);
    }
}