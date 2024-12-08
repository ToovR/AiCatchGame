namespace AiCatchGame.Bo
{
    public class Player
    {
        public Player(string privateId)
        {
            PrivateId = privateId;
        }

        public CharacterInfo? Character { get; set; }
        public GameStatuses GameStatus { get; set; }
        public string PrivateId { get; }
        public PlayerStatuses Status { get; set; }
    }
}