using AiCatchGame.Bo.Exceptions;
using AiCatchGame.Web.Client.Interfaces;
using AiCatchGame.Web.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Diagnostics;

namespace AiCatchGame.Web.Client.Components.Pages
{
    public partial class Home
    {
        public string? PlayerPseudonym { get; set; } = "";

        [Inject]
        public IPlayerService? PlayerService { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IHubClientService? HubClientService { get; set; }

        [Inject]
        private IJSRuntime? JsRuntime { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        public async Task SubmitPlayer()
        {
            ArgumentNullException.ThrowIfNull(JsRuntime);
            ArgumentNullException.ThrowIfNull(NavigationManager);
            ArgumentNullException.ThrowIfNull(HubClientService);
            ArgumentNullException.ThrowIfNull(PlayerService);
            ArgumentNullException.ThrowIfNull(HttpClient);

            string url = $"{HttpClient.BaseAddress}gameHub";
            Trace.WriteLine($"url : {url}");
            HubClientService.Initialize(url);

            await JsRuntime.InvokeVoidAsync("autoplay");

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