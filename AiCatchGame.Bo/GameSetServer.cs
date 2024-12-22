namespace AiCatchGame.Bo
{
    public class GameSetServer(int roundNumber, Guid id)
    {
        public Guid Id { get; } = id;
        public List<PlayerSetInfo> PlayerSetInfoList { get; set; } = [];
        public int RoundNumber { get; } = roundNumber;

        public GameSetStatuses Status { get; set; }
    }
}