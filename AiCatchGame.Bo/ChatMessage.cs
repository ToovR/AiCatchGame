namespace AiCatchGame.Bo
{
    public class ChatMessage
    {
        public ChatMessage(CharacterInfo character, string content)
        {
            Character = character;
            Content = content;
        }

        public CharacterInfo Character { get; set; }
        public string Content { get; }
        public DateTime? Time { get; set; }
    }
}