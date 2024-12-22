namespace AiCatchGame.Bo
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string privateId, string pseudonym) : base(privateId)
        {
            Pseudonym = pseudonym;
            PublicId = Guid.NewGuid();
        }

        public string Pseudonym { get; }
        public Guid PublicId { get; }
        public List<HumanPlayerGameSetResultInfo> SetResults { get; set; } = [];
    }
}