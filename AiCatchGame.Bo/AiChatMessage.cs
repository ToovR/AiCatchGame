namespace AiCatchGame.Bo
{
    public class AiChatMessage
    {
        public AiChatMessage(string playerId, string content, double delay)
        {
            PlayerId = playerId;
            Content = content;
            Delay = delay;
        }

        public string Content { get; }
        public double Delay { get; }
        public string PlayerId { get; }
    }
}