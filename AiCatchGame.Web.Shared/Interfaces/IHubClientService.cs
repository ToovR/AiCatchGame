using AiCatchGame.Bo;

namespace AiCatchGame.Web.Shared.Interfaces
{
    public interface IHubClientService
    {
        void Initialize(string url);

        Task OnGameJoined(Func<string, Guid, Task> handler);

        Task OnGameStart(Func<GameClient, Task> gameAction);

        Task OnNewPlayer(Func<string, Task> onNewPlayer);

        Task OnReceivedMessage(Func<ChatMessage, Task> receivedMessageAction);

        Task OnSetShowScore(Func<GameSetResultInfo, Task> setShowScoreAction);

        Task OnSetStart(Func<GameSetClient, Task> setStartAction);

        Task OnSetStartChat(Func<GameSetChattingInfo, Task> setStartChatAction);

        Task OnSetStartVote(Func<GameSetVotingInfo, Task> setStartVoteAction);

        Task SendMessage(string playerId, string message);

        Task SendPlayerReady(string playerId);

        Task StartJoinGame(string pseudonym);

        Task Vote(string playerId, Guid characterVotedId);
    }
}