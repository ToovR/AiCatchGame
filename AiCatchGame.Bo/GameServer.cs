namespace AiCatchGame.Bo
{
    public class GameServer
    {
        public List<AiPlayer> AiPlayers { get; set; } = [];
        public List<GameSet> GameSets { get; set; }
        public DateTime GameStartedTime { get; set; }
        public List<HumanPlayer> HumanPlayers { get; set; } = [];
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime LastAddedPlayerTime { get; set; }
        public IEnumerable<string> PlayerIds
        { get { return HumanPlayers.Select(p => p.PrivateId); } }
        public GameStatuses Status { get; set; }
    }
}