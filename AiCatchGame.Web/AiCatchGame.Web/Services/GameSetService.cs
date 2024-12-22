using AiCatchGame.Bo;
using AiCatchGame.Web.Helpers;
using AiCatchGame.Web.Interfaces;

namespace AiCatchGame.Web.Services
{
    public class GameSetService : IGameSetService
    {
        private readonly IGameService _game;

        private List<string> _poolCharacterNames = [ "Luke", "Leia", "Han", "Chewbacca", "Obi-Wan", "Anakin", "Padmé", "Yoda",
            "Darth Vader", "R2-D2", "C-3PO", "Lando", "Boba Fett", "Jabba", "Wedge",
            "Mace Windu", "Qui-Gon Jinn", "Darth Maul", "Jar Jar Binks", "Count Dooku",
            "General Grievous", "Ahsoka Tano", "Rey", "Kylo Ren", "Finn", "Poe Dameron",
            "BB-8", "Maz Kanata", "Snoke", "Rose Tico", "Jyn Erso", "Cassian Andor",
            "K-2SO", "Chirrut Îmwe", "Baze Malbus", "Saw Gerrera", "Orson Krennic",
            "Bodhi Rook", "Galen Erso", "Mon Mothma", "Bail Organa", "Jango Fett",
            "Darth Sidious", "Darth Tyranus", "Darth Plagueis", "Darth Bane", "Darth Revan",
            "Darth Malak", "Darth Nihilus", "Darth Sion", "Darth Traya", "Darth Malgus"];

        public GameSetService(IGameService game)
        {
            _game = game;
        }

        public Task EndChatPhase(Guid gameSetId)
        {
            GameSetServer gameSet = GetGameSet(gameSetId);
            gameSet.Status = GameSetStatuses.Voting;
            throw new NotImplementedException();
            // TODO Notify vote
        }

        public Task EndSetPhase(Guid gameSetId)
        {
            throw new NotImplementedException();
            // TODO Calculate scores
            // TODO calculate losers
            // TODO determine if game is over

            // TODO Notify vote
        }

        public GameSetServer GetGameSet(Guid id)
        {
            IEnumerable<GameServer> games = _game.GetGames();
            return games.SelectMany(g => g.GameSets).Single(gs => gs.Id == id);
        }

        public Task<GameSetServer> InitializeSet(GameServer game)
        {
            int roundNumber = (game.GameSets.MaxOrDefault(s => s.RoundNumber) ?? 0) + 1;
            GameSetServer newGameSet = new(roundNumber, Guid.NewGuid());
            game.GameSets.Add(newGameSet);
            List<Player> remainingPlayers = game.HumanPlayers.Cast<Player>().Concat(game.AiPlayers.Cast<Player>()).Where(p => p.Status != PlayerStatuses.Lost).ToList();

            List<string> remainingCharacterNames = ((string[])_poolCharacterNames.ToArray().Clone()).ToList();
            remainingPlayers.ForEach(p =>
            {
                p.GameStatus = GameStatuses.Playing;
                int index = (new Random()).Next(remainingCharacterNames.Count);

                string characterName = remainingCharacterNames[index];
                remainingCharacterNames.RemoveAt(index);
                newGameSet.PlayerSetInfoList.Add(new PlayerSetInfo(p.PrivateId, characterName, Guid.NewGuid()));
            });

            newGameSet.Status = GameSetStatuses.Chatting;
            // TODO Initialize Timer of chat
            // TODO Notify set start
            return null;
        }
    }
}