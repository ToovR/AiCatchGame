namespace AiCatchGame.Bo
{
    public class GameSetClient(Guid gameId, PlayerSetInfo playerSetInfo, CharacterInfo[] characters, int roundNumber, GameSetStatuses status, int characterAttributionDuration)
    {
        public int CharacterAttributionDuration { get; } = characterAttributionDuration;
        public CharacterInfo[] Characters { get; } = characters;
        public Guid GameId { get; } = gameId;
        public PlayerSetInfo PlayerSetInfo { get; } = playerSetInfo;
        public int RoundNumber { get; } = roundNumber;
        public GameSetStatuses Status { get; set; } = status;
    }
}