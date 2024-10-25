using AiCatchGame.Bo.Exceptions;
using AiCatchGame.Web.Client.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AiCatchGame.Web.Client.Pages
{
    public partial class Home
    {
        public string? PlayerPseudonym { get; set; }

        [Inject]
        public IPlayerService? PlayerService { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        public async Task SubmitPlayer()
        {
            ArgumentNullException.ThrowIfNull(NavigationManager);
            ArgumentNullException.ThrowIfNull(PlayerService);

            if (String.IsNullOrWhiteSpace(PlayerPseudonym))
            {
                _snackBar.Add("Le pseudonyme ne peut pas être vide", Severity.Error);
                return;
            }

            ErrorCodes error = await PlayerService.AddPlayer(PlayerPseudonym);
            if (error == ErrorCodes.AlreadyExists)
            {
                _snackBar.Add($"Le pseudonyme {PlayerPseudonym} est déjà utilisé", Severity.Error);
                return;
            }
            if (error != ErrorCodes.None)
            {
                _snackBar.Add($"Erreur indéfinie rencontrée lors de l'ajout du joueur", Severity.Error);
                return;
            }
            NavigationManager.NavigateTo($"/game");
        }
    }
}