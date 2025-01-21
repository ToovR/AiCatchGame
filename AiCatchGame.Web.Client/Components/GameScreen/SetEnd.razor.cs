using AiCatchGame.Bo;
using Microsoft.AspNetCore.Components;

namespace AiCatchGame.Web.Client.Components.GameScreen
{
    public partial class SetEnd
    {
        private Dictionary<Guid, string>? _characters;
        private CharacterInfo? _currentCharacter;

        [Parameter]
        public GameSetClient? CurrentSet { get; set; }

        [Parameter]
        public GameSetResultInfo? SetResult { get; set; }

        protected string GetPlayersStringById(Guid[] playerIds)
        {
            ArgumentNullException.ThrowIfNull(SetResult);
            string[] names = playerIds.Select(id => SetResult.HumanPlayers.Single(p => p.PlayerId == id).Pseudonym).ToArray();
            if (names.Length == 0)
            {
                return "Personne";
            }
            if (names.Length == 1)
            {
                return names[0];
            }
            return $"{string.Join(", ", names.Take(names.Length - 1))} et {names.Last()}";
        }

        protected override async Task OnInitializedAsync()
        {
            ArgumentNullException.ThrowIfNull(CurrentSet);
            _characters = CurrentSet.Characters.ToDictionary(c => c.Id, c => c.Name);
            _currentCharacter = new(CurrentSet.PlayerSetInfo.CharacterId, CurrentSet.PlayerSetInfo.CharacterName);
            await base.OnInitializedAsync();
        }

        private string GetCharacterNameById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(_characters);
            return _characters[id];
        }
    }
}