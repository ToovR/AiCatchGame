using AiCatchGame.Bo;

namespace AiCatchGame.Web.Client.Interfaces
{
    public interface IHubClientService
    {
        Task OnGameJoined(Func<string, Guid, Task> handler);

        Task OnGameStart(Func<GameClient, Task> gameAction);

        Task OnNewPlayer(Func<string, Task> onNewPlayer);

        Task OnReceivedMessage(Func<Guid, string, Task> receivedMessageAction);

        Task OnSetShowScore(Func<GameSetResultInfo, Task> setShowScoreAction);

        Task OnSetStart(Func<GameSetClient, Task> setStartAction);

        Task OnSetStartVote(Func<GameSetVotingInfo, Task> setStartVoteAction);

        Task SendMessage(string playerId, string message);

        Task SendPlayerReady(string playerId);

        Task StartJoinGame(string pseudonym);

        Task Vote(string playerId, Guid characterVotedId);
    }
}