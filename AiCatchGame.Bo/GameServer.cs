namespace AiCatchGame.Bo
{
    public class GameServer
    {
        public GameServer(GameRuleInfo ruleInfo)
        { Rules = ruleInfo; }

        public List<AiPlayer> AiPlayers { get; set; } = [];
        public List<GameSetServer> GameSets { get; set; } = [];
        public DateTime GameStartedTime { get; set; }
        public List<HumanPlayer> HumanPlayers { get; set; } = [];
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime LastAddedPlayerTime { get; set; }

        public IEnumerable<string> PlayerPrivateIds
        { get { return HumanPlayers.Select(p => p.PrivateId); } }

        public GameRuleInfo Rules { get; }
        public GameStatuses Status { get; set; }
    }
}