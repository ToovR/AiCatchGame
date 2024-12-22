namespace AiCatchGame.Bo
{
    public class GameSetServer(int roundNumber, Guid id)
    {
        public DateTime? ChattingStartTime { get; set; } = null;
        public Guid Id { get; } = id;
        public List<PlayerSetInfo> PlayerSetInfoList { get; set; } = [];
        public int RoundNumber { get; } = roundNumber;
        public GameSetStatuses Status { get; set; }
        public List<VoteInfo> Votes { get; set; } = [];
        public DateTime? VotingStartTime { get; set; } = null;
    }
}