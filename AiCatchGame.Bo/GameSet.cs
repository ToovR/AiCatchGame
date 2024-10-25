namespace AiCatchGame.Bo
{
    public class GameSet
    {
        public GameSet(int roundNumber, Guid id)
        {
            RoundNumber = roundNumber;
            Id = id;
        }

        public Guid Id { get; }
        public int RoundNumber { get; }

        public GameSetStatuses Status { get; set; }
    }
}