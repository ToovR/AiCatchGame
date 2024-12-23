namespace AiCatchGame.Bo
{
    public class GameSetClient(PlayerSetInfo playerSetInfo, CharacterInfo[] characters, int roundNumber, GameSetStatuses status, int characterAttributionDuration)
    {
        public int CharacterAttributionDuration { get; } = characterAttributionDuration;
        public CharacterInfo[] Characters { get; } = characters;
        public PlayerSetInfo PlayerSetInfo { get; } = playerSetInfo;
        public int RoundNumber { get; } = roundNumber;
        public GameSetStatuses Status { get; set; } = status;
    }
}