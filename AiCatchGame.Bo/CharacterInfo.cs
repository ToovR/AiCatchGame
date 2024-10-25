namespace AiCatchGame.Bo
{
    public class CharacterInfo
    {
        public CharacterInfo(Guid id, string characterName)
        {
            Id = id;
            Name = characterName;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}