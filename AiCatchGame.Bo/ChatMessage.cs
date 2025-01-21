namespace AiCatchGame.Bo
{
    public record ChatMessage(Guid CharacterId, string Content, DateTime Time);
}