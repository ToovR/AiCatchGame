namespace AiCatchGame.Bo
{
    public class GameSetResultInfo
    {
        public AiGameSetResultInfo[] AiInfoList { get; set; } = [];
        public IEnumerable<HumanPlayerGameSetResultInfo> HumanPlayers { get; set; } = [];
    }
}