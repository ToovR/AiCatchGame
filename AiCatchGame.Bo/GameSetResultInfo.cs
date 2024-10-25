namespace AiCatchGame.Bo
{
    public class GameSetResultInfo
    {
        public AiGameSetResultInfo AiInfo { get; set; }
        public IEnumerable<PlayerGameSetResultInfo> Players { get; set; } = [];
    }
}