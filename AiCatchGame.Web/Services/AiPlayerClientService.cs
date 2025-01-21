using AiCatchGame.Bo;
using AiCatchGame.Web.Interfaces;
using AiCatchGame.Web.Shared.Interfaces;

namespace AiCatchGame.Web.Services
{
    public class AiPlayerClientService
    {
        //   private readonly AiPlayerService _aiPlayerService;
        private readonly Dictionary<Guid, GameSetClient> _gameSets = [];

        private readonly IServiceScopeFactory _serviceScopeFactory;

        // private readonly IHubClientService _hubClientService;

        public AiPlayerClientService(Bo.AiPlayer aiPlayer, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void InitializeClient(string url, Bo.AiPlayer aiPlayer)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IHubClientService hubClientService = scope.ServiceProvider.GetRequiredService<IHubClientService>();

            hubClientService.Initialize(url);
            hubClientService.OnSetStart(OnSetStart);
            hubClientService.OnReceivedMessage(OnReceivedMessage);
            hubClientService.OnSetStartChat(OnSetStartChat);
        }

        public async Task OnReceivedMessage(ChatMessage message)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IAiPlayerService aiPlayerService = scope.ServiceProvider.GetRequiredService<IAiPlayerService>();

            await aiPlayerService.OnPlayerSpeak(message);
        }

        public Task OnSetStart(GameSetClient gameSet)
        {
            _gameSets[gameSet.GameId] = gameSet;
            return Task.CompletedTask;
        }

        private Task OnSetStartChat(GameSetChattingInfo info)
        {
            return Task.CompletedTask;
        }
    }
}