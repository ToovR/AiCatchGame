namespace AiCatchGame.Bo
{
    public class Player
    {
        public Guid Id { get; } = Guid.NewGuid();
        public PlayerStatuses Status { get; set; }
    }
}