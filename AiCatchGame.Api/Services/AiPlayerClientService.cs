//using AiCatchGame.Api.Interfaces;
//using AiCatchGame.Bo;
//using AiCatchGame.Web.Shared.Interfaces;

//namespace AiCatchGame.Api.Services
//{
//    public class AiPlayerClientService
//    {
//        //   private readonly AiPlayerService _aiPlayerService;
//        private readonly Dictionary<Guid, GameSetClient> _gameSets = [];

//        private readonly IServiceScopeFactory _serviceScopeFactory;

//        // private readonly IHubClientService _hubClientService;
//        private IHubClientService? _hubClientService;
//        public AiPlayerClientService(Bo.AiPlayer aiPlayer, IServiceScopeFactory serviceScopeFactory)
//        {
//            _serviceScopeFactory = serviceScopeFactory;
//        }

//        public void InitializeClient(string url, Bo.AiPlayer aiPlayer)
//        {
//            try
//            {
//                IServiceScope scope = _serviceScopeFactory.CreateScope();
//                _hubClientService = scope.ServiceProvider.GetRequiredService<IHubClientService>();

//                _hubClientService.Initialize(url);
//                _hubClientService.OnSetStart(OnSetStart);
//                _hubClientService.OnReceivedMessage(OnReceivedMessage);
//                _hubClientService.OnSetStartChat(OnSetStartChat);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//        }

//        public async Task OnReceivedMessage(ChatMessage message)
//        {
//            using IServiceScope scope = _serviceScopeFactory.CreateScope();
//            IAiPlayerService aiPlayerService = scope.ServiceProvider.GetRequiredService<IAiPlayerService>();

//            await aiPlayerService.OnPlayerSpeak(message);
//        }

//        public Task OnSetStart(GameSetClient gameSet)
//        {
//            _gameSets[gameSet.GameId] = gameSet;
//            return Task.CompletedTask;
//        }

//        private Task OnSetStartChat(GameSetChattingInfo info)
//        {
//            return Task.CompletedTask;
//        }
//    }
//}