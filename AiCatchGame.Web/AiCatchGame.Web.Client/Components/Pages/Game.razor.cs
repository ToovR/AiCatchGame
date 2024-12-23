using AiCatchGame.Bo;
using AiCatchGame.Web.Client.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AiCatchGame.Web.Client.Components.Pages
{
    public partial class Game
    {
        private readonly List<string> _lobbyMessages = [];
        private Dictionary<Guid, string>? _characters;
        private CharacterInfo? _currentCharacter;
        private string _message = "";
        private List<string> _messages = [];
        private CharacterInfo[]? _otherCharacters;
        private GameSetResultInfo? _setResult;
        public GameSetClient? CurrentSet { get; set; }
        public Guid? GameId { get; set; }
        public Guid? PlayerId { get; set; }
        private AiCatchGame.Bo.GameClient? GameData { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Inject]
        private IPlayerService? PlayerService { get; set; }

        private int TimerRemaining { get; set; }

        protected string GetPlayersStringById(Guid[] playerIds)
        {
            ArgumentNullException.ThrowIfNull(_setResult);
            string[] names = playerIds.Select(id => _setResult.HumanPlayers.Single(p => p.PlayerId == id).Pseudonym).ToArray();
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
            ArgumentNullException.ThrowIfNull(NavigationManager);
            ArgumentNullException.ThrowIfNull(PlayerService);
            Guid? gameId = await PlayerService.GetGameId();
            if (gameId == null)
            {
                NavigationManager.NavigateTo("/");
            }

            await InitializeLobbyState();
        }

        private Task AddMessage(Guid characterId, string message)
        {
            ArgumentNullException.ThrowIfNull(_characters);
            ArgumentNullException.ThrowIfNull(_messages);
            string encodedMsg = $"{_characters[characterId]}: {message}";
            _messages.Add(encodedMsg);
            this.StateHasChanged();
            return Task.CompletedTask;
        }

        private string GetCharacterNameById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(_characters);
            return _characters[id];
        }

        private Task InitializeChat(GameSetChattingInfo chatInfo)
        {
            ArgumentNullException.ThrowIfNull(CurrentSet);
            _messages = [];
            CurrentSet.Status = GameSetStatuses.Chatting;
            TimerRemaining = chatInfo.Duration;
            return Task.CompletedTask;
        }

        private async Task InitializeLobbyState()
        {
            ArgumentNullException.ThrowIfNull(PlayerService);
            GameData = new GameClient(GameStatuses.InLobby);
            await PlayerService.OnGameStart((Bo.GameClient data) =>
            {
                GameData = data;
                this.StateHasChanged();
                return Task.CompletedTask;
            });
            await PlayerService.OnSetStart(InitializeSet);
            await PlayerService.OnSetStartChat(InitializeChat);
            await PlayerService.OnSetStartVote(InitializeVote);
            await PlayerService.OnShowScore(InitializeShowScore);
            await PlayerService.OnSetSomeoneVoted(OnSomeoneVoted);
            await PlayerService.OnReceivedMessage(AddMessage);
            await PlayerService.OnNewPlayer((string pseudonym) =>
            {
                _lobbyMessages.Add($"{pseudonym} s'est joint à la partie.");
                this.StateHasChanged();
                return Task.CompletedTask;
            });

            await PlayerService.NotifyReady();
        }

        private Task InitializeSet(GameSetClient gameSet)
        {
            ArgumentNullException.ThrowIfNull(PlayerService);
            CurrentSet = gameSet;
            _currentCharacter = new(gameSet.PlayerSetInfo.CharacterId, gameSet.PlayerSetInfo.CharacterName);
            _characters = gameSet.Characters.ToDictionary(c => c.Id, c => c.Name);
            TimerRemaining = 10;
            CurrentSet.Status = GameSetStatuses.CharacterAttribution;
            this.StateHasChanged();
            return Task.CompletedTask;
        }

        private Task InitializeShowScore(GameSetResultInfo resultInfo)
        {
            ArgumentNullException.ThrowIfNull(CurrentSet);
            _setResult = resultInfo;
            CurrentSet.Status = GameSetStatuses.End;
            this.StateHasChanged();
            return Task.CompletedTask;
        }

        private Task InitializeVote(GameSetVotingInfo voteInfo)
        {
            ArgumentNullException.ThrowIfNull(_currentCharacter);
            ArgumentNullException.ThrowIfNull(CurrentSet);
            TimerRemaining = voteInfo.Duration;
            _otherCharacters = voteInfo.Characters.Where(c => c.Id != _currentCharacter.Id).ToArray();
            CurrentSet.Status = GameSetStatuses.Voting;
            this.StateHasChanged();
            return Task.CompletedTask;
        }

        private Task OnSomeoneVoted(SomeoneVotedInfo someoneVotedInfo)
        {
            _snackBar.Add($"{someoneVotedInfo.PlayerName} a voté", Severity.Info);
            return Task.CompletedTask;
        }

        private async Task SendMessage()
        {
            ArgumentNullException.ThrowIfNull(PlayerService);
            await PlayerService.SendMessage(_message);
        }

        private async Task VoteFor(Guid characterId)
        {
            ArgumentNullException.ThrowIfNull(PlayerService);
            await PlayerService.Vote(characterId);
        }
    }
}