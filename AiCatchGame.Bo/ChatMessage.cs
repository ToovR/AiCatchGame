namespace AiCatchGame.Bo
{
    public class ChatMessage
    {
        public ChatMessage(CharacterInfo character, string content)
        {
            Character = character;
            Content = content;
        }

        public string Content { get; }
        public CharacterInfo Character { get; set; }
        public DateTime? Time { get; set; }
    }
}