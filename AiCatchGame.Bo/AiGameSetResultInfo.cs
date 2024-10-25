namespace AiCatchGame.Bo
{
    public class AiGameSetResultInfo
    {
        public AiGameSetResultInfo(CharacterInfo character)
        {
            Character = character;
        }

        public CharacterInfo Character { get; set; }
    }
}