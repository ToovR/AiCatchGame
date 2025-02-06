namespace AiCatchGame.Bo
{
    public class AiChatMessage
    {
        public AiChatMessage(string playerId, Guid characterId, string content, double delay)
        {
            PlayerId = playerId;
            CharacterId = characterId;
            Content = content;
            Delay = delay;
        }

        public Guid CharacterId { get; }
        public string Content { get; }
        public double Delay { get; }
        public string PlayerId { get; }
    }
}