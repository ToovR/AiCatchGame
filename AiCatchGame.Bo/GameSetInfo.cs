namespace AiCatchGame.Bo
{
    public class GameSetInfo
    {
        public CharacterInfo[] Characters { get; set; } = [];
        public Guid Id { get; set; }
        public int RoundNumber { get; set; }
    }
}