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

        void OnGameStart(Action<GameClient> gameAction);

        void OnNewPlayer(Action<string> onNewPlayer);

        void OnReceivedMessage(Action<Guid, string> receivedMessageAction);

        Task OnSetEnd(Func<GameSetResultInfo, PlayerGameSetResultInfo, Task> setEndAction);

        void OnSetSomeoneVoted(Action<SomeoneVotedInfo> someoneVotedAction);

        void OnSetStart(Action<GameSetClient> setStartAction);

        void OnSetStartChat(Action<GameSetChattingInfo> setStartChatAction);

        void OnSetStartVote(Action<GameSetVotingInfo> setStartVoteAction);

        Task SendMessage(string message);

        Task Vote(Guid characterVotedId);
    }
}