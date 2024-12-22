namespace AiCatchGame.Bo
{
    public class GameRuleInfo
    {
        /// <summary>
        /// Get or set chat time duration in seconds
        /// </summary>
        public int ChatDuration { get; set; }

        public int PlayerMax { get; set; }

        public int PlayerMin { get; set; }

        public int ScoreVotedFor { get; set; }
        public int ScoreVoteMax { get; set; }

        /// <summary>
        /// Get or set vote time duration in seconds
        /// </summary>
        public int VoteDuration { get; set; }

        public static GameRuleInfo GetDefault()
        {
            return new GameRuleInfo()
            {
                ChatDuration = 120,
                PlayerMin = 2,
                PlayerMax = 10,
                ScoreVotedFor = 400,
                ScoreVoteMax = 1000,
                VoteDuration = 10,
            };
        }
    }
}