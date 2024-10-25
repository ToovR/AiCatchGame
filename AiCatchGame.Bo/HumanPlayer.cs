namespace AiCatchGame.Bo
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string privateId, string pseudonym)
        {
            PrivateId = privateId;
            Pseudonym = Pseudonym;
            PublicId = Guid.NewGuid();
        }

        public string PrivateId { get; }
        public string Pseudonym { get; }
        public Guid PublicId { get; }
    }
}