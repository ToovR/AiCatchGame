namespace AiCatchGame.Bo
{
    public class ChatMessage
    {
        public ChatMessage(string content, double delay)
        {
            Content = content;
            Delay = delay;
        }

        public string Content { get; }
        public double Delay { get; }
        public Player? Player { get; set; }
        public DateTime? Time { get; set; }
    }
}