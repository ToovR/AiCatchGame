using AiCatchGame.Bo;
using AiCatchGame.Bo.Exceptions;

namespace AiCatchGame.Web.Client.Interfaces
{
    public interface IPlayerService
    {
        Task<ErrorCodes> AddPlayer(string pseudonym);

        Task<CharacterInfo> GetCharacterInfo(Guid id);

        Task<Guid?> GetGameId();

        Task NotifyReady();

        Task OnGameStart(Func<GameClient, Task> gameAction);

        Task OnNewPlayer(Func<string, Task> onNewPlayer);

        Task OnReceivedMessage(Func<Guid, string, Task> receivedMessageAction);

        Task OnSetSomeoneVoted(Func<SomeoneVotedInfo, Task> someoneVotedAction);

        Task OnSetStart(Func<GameSetClient, Task> setStartAction);

        Task OnSetStartChat(Func<GameSetChattingInfo, Task> setStartChatAction);

        Task OnSetStartVote(Func<GameSetVotingInfo, Task> setStartVoteAction);

        Task OnShowScore(Func<GameSetResultInfo, Task> setEndAction);

        Task SendMessage(string message);

        Task Vote(Guid characterVotedId);
    }
}