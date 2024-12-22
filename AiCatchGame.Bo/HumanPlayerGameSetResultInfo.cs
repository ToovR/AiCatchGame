namespace AiCatchGame.Bo
{
    public class HumanPlayerGameSetResultInfo
    {
        public Guid CharacterId { get; set; }
        public bool HasFoundAi { get; set; }
        public Guid PlayerId { get; set; }
        public Guid[] PlayerIdsVotedFor { get; set; } = [];
        public string Pseudonym { get; set; } = "";
        public int ScoreSet { get; set; }
        public int ScoreTotal { get; set; }

        /// <summary>
        /// Score depend on VOte
        /// </summary>
        public int ScoreVote { get; set; }

        public int ScoreVotedAsAi { get; set; }
        public PlayerStatuses Status { get; set; }
        public Guid VotedCharacterId { get; set; }
        public double? VoteReaction { get; set; }
    }
}