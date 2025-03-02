using AiCatchGame.Api.Interfaces;
using AiCatchGame.Bo;
using AiCatchGame.Web.Helpers;

namespace AiCatchGame.Api.Services
{
    public class GameSetService : IGameSetService
    {
        private readonly IGameService _game;

        private List<string> _poolCharacterNames = [ "Luke", "Leia", "Han", "Chewbacca", "Obi-Wan", "Anakin", "Padmé", "Yoda",
            "Darth Vader", "R2-D2", "C-3PO", "Lando", "Boba Fett", "Jabba", "Qui-Gon Jinn", "Darth Maul", "Jar Jar Binks", "Count Dooku",
            "General Grievous", "Rey"];

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
            List<Player> players = game.HumanPlayers.Cast<Player>().Concat(game.AiPlayers.Cast<Player>()).ToList();

            List<string> characterNames = ((string[])_poolCharacterNames.ToArray().Clone()).ToList();
            players.ForEach(p =>
            {
                p.GameStatus = GameStatuses.Playing;
                int index = (new Random()).Next(characterNames.Count);

                string characterName = characterNames[index];
                characterNames.RemoveAt(index);
                newGameSet.PlayerSetInfoList.Add(new PlayerSetInfo(p.PrivateId, characterName, Guid.NewGuid(), p is AiPlayer));
            });

            newGameSet.Status = GameSetStatuses.CharacterAttribution;
            newGameSet.CharacterAttributionStartTime = DateTime.Now;
            return Task.FromResult(newGameSet);
        }
    }
}