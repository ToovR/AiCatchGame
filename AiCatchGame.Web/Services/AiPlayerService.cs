using AiCatchGame.Bo;
using AiCatchGame.Web.Interfaces;

namespace AiCatchGame.Web.Services
{
    public class AiPlayerService : IAiPlayerService
    {
        private List<AiPlayer> _aiPlayers = [];
        private IChatService _chatService;
        private DateTime _lastMessageTime;
        private List<ChatMessage> _messages = [];
        private Queue<AiChatMessage> _messagesToSend = [];
        private Queue<string> _responses = [];
        private Dictionary<Guid, AiPlayerClientService> clientServices = [];

        private string defaultSystemMessage =
        """
Tu joues à un jeu avec d'autres joueurs qui sont des êtres humains. Le jeu consiste pour toi à te faire passer pour un humain et aux humains de deviner qu tu es un IA.
Les joueurs ont l'ensemble des échanges sous forme de chat. Tu es sensé pouvoir savoir ce qui a été écrit précédemment.
Je vais te relayer les phrases des autres êtres humains? De ton coté, tu dois me fournir des phrases qui te feront passer pour un être humain. Par ailleurs, chaque joueur porte le prénom d'un personnage de Star Wars.

Il ne faut pas toujours parler. Quand les autres joueurs se parlent les uns les autres évite de répondre. Marque "[Ne répond pas]" à la place.

Ne commence pas tes phrases par Salut
met quelques fautes d orthographe dans tes réponses.

Pour certaines informations pas évidentes pour un humain à donner comme ca, tu dois faire croire que tu n'as pas la réponse sous la main
dans chacune de tes réponses, inclut tout au debut une indication sur le temps qu'un humain mettrait à écrire la réponse sous la forme [TEMPS:XXs]
""";

        public AiPlayerService(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task ManageResponse()
        {
            while (true)
            {
                while (_messagesToSend.Count > 0)
                {
                    AiChatMessage message = _messagesToSend.Dequeue();
                    double timeToWait = (DateTime.Now - _lastMessageTime).TotalSeconds - message.Delay;
                    timeToWait = timeToWait >= 0 ? timeToWait : 0;

                    using System.Threading.Timer t = new System.Threading.Timer((e) =>
                    {
                        string? treatedResponse = TreatResponse(message);
                        if (treatedResponse == null)
                        {
                            return;
                        }

                        _chatService.PostMessage(message.PlayerId, message.Content);
                    }, null, 0, (int)(timeToWait * 1000));
                }
                await Task.Delay(200);
            }
        }

        public void OnGameInitialize()
        {
        }

        public async Task OnPlayerSpeak(ChatMessage message)
        {
            _lastMessageTime = message.Time;
            _messages.Add(message);
            // TODO Add to AI LLM received message and get response
            foreach (AiPlayer player in _aiPlayers)
            {
                string response = await TreatInAi(player, message);

                double delay = DefineDelay(message);
                AiChatMessage aiChatMessage = new AiChatMessage(player.PrivateId, response, delay);
                _messagesToSend.Enqueue(aiChatMessage);
            }
        }

        private double DefineDelay(ChatMessage message)
        {
            // TODO Define delay while considering other players delay
            return ((double)new Random().Next(500, 2000)) / 1000;
        }

        private Task<string> TreatInAi(AiPlayer player, ChatMessage message)
        {
            string response = new Random().Next(0, 2) == 0 ? "Message Test" : "[Ne répond pas]";
            return Task.FromResult(response);
        }

        private string? TreatResponse(AiChatMessage message)
        {
            if (message.Content.Contains("[Ne répond pas]"))
            {
                return null;
            }
            return message.Content;
        }
    }
}