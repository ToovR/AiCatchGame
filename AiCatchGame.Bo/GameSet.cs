namespace AiCatchGame.Bo
{
    public class GameSet
    {
        public GameSet(int roundNumber, Guid id)
        {
            RoundNumber = roundNumber;
            Id = id;
            PlayerSetInfoList = [];
        }

        public Guid Id { get; }
        public List<PlayerSetInfo> PlayerSetInfoList { get; set; }
        public int RoundNumber { get; }

        public GameSetStatuses Status { get; set; }
    }
}