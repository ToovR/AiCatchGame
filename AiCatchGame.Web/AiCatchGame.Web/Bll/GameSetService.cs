namespace AiCatchGame.Web.Bll
{
    public class GameSet
    {
        private List<string> _poolNames = [ "Luke", "Leia", "Han", "Chewbacca", "Obi-Wan", "Anakin", "Padmé", "Yoda",
            "Darth Vader", "R2-D2", "C-3PO", "Lando", "Boba Fett", "Jabba", "Wedge",
            "Mace Windu", "Qui-Gon Jinn", "Darth Maul", "Jar Jar Binks", "Count Dooku",
            "General Grievous", "Ahsoka Tano", "Rey", "Kylo Ren", "Finn", "Poe Dameron",
            "BB-8", "Maz Kanata", "Snoke", "Rose Tico", "Jyn Erso", "Cassian Andor",
            "K-2SO", "Chirrut Îmwe", "Baze Malbus", "Saw Gerrera", "Orson Krennic",
            "Bodhi Rook", "Galen Erso", "Mon Mothma", "Bail Organa", "Jango Fett",
            "Darth Sidious", "Darth Tyranus", "Darth Plagueis", "Darth Bane", "Darth Revan",
            "Darth Malak", "Darth Nihilus", "Darth Sion", "Darth Traya", "Darth Malgus"];

        public List<GamSetPlayer> PlayerSetInfos { get; set; }

        public async Task EndChatPhase(Guid gameSetId)
        {
            GameSet gameSet = getset(gameSetId);
            gameSet.Status = SetPhase.Vote;
            // TODO Notify vote
        }

        public async Task EndSetPhase(Guid gameSetId)
        {
            // TODO Calculate scores
            // TODO calculate losers
            // TODO determine if game is over

            // TODO Notify vote
        }

        public async Task InitializeSet(Game game)
        {
            int roundNumber = (game.Sets.MaxOrDefault(s => s.RoundNumber) ?? 0) + 1;
            GameSet newGameSet = new(rounNumber);
            List<Player> remainingPlayers = game.HumanPlayers.Concat(game.Aiplayer).Cast<Players>().Where(p => p.GameStatus != GameStatuses.Lost).ToList();

            string[] poolNames = _poolNames.CloneArray();
            remainingPlayers.ForEach(p =>
            {
                p.GameStatus = GameStatuses.Playing;
                int index = Random.Next(poolNames.length)

                string characterName = poolNames[index];
                poolNames.RemoveAt(index);
                p.Sets.Add(new PlayerSetInfo(newGameSet, characterName));
            });

            newGameSet.Status = SetStatuses.ChatPhase;
            // TODO Initialize Timer of chat
            // TODO Notify set start
        }
    }
}