using AiCatchGame.Bo;
using System.Timers;

namespace AiCatchGame.Web.Bll
{
    public class AiPlayerService
    {
        private ChatService _chatService;

        private DateTime _lastMessageTime;
        private List<ChatMessage> _messages = [];
        private Queue<ChatMessage> _messagesToSend = [];
        private Queue<string> _responses = [];

        private string defaultSystemMessage =
        """
Tu joues à un jeu avec d'autres joueurs qui sont des etres humains. Le jeu consiste pour toi à te faire passer pour un humain et aux humains de deviner qu tu es un IA.
Les joueurs ont l'ensemble des echanges sous forme de chat. Tu es sensé pouvoir savoir ce qui a été écrit précédement.
Je vais te relayer les phrases des autres etres humains? De ton coté, tu dois me fournir des phrases qui te feront passer pour un etre humain. Par ailleurs, chaque joueur porte le prénom d'un personnage de Star Wars.

Il ne faut pas toujours parler. Quand les autres joueurs se parlent les uns les autres evite de répondre. Marque "[Ne répond pas]" à la place.

Ne commence pas tes phrases par Salut
met quelques fautes d orthographe dans tes réponses.

Pour certaines informations pas évidentes pour un humain à donner comme ca, tu dois faire croire que tu n'as pas la réponse sous la main
dans chacune de tes réponses, inclut tout au debut une indication sur le temps qu'un humain mettrait à écrire la réponse sous la forme [TEMPS:XXs]
""";

        public async Task ManageResponse()
        {
            while (true)
            {
                while (_messagesToSend.Count > 0)
                {
                    ChatMessage message = _messagesToSend.Dequeue();
                    double timeToWait = (DateTime.Now - _lastMessageTime).TotalSeconds - message.Delay;
                    timeToWait = timeToWait >= 0 ? timeToWait : 0;

                    using System.Threading.Timer t = new System.Threading.Timer((e) =>
                    {
                        string? treatedResponse = TreatResponse(message);
                        if (treatedResponse == null)
                        {
                            return;
                        }
                        DefineDelay(message);
                        _messagesToSend.Enqueue(new ChatMessage(treatedResponse, timeToWait));
                    }, null, 0, (int)(timeToWait * 1000));
                }
                await Task.Delay(200);
            }
        }

        public void OnGameInitialize()
        {
        }

        public void OnPlayerSpeak(ChatMessage message)
        {
            _lastMessage = message.Time;
            _messages.Add(message);

            string reponse = _chatService.Chat($"{message.Player.Name} a dit : \"{message.Content}\"");
            _responses.Add(reponse);
        }

        public void OnStartSet()
        {
            _chatService = new();
            _chatService.Initialize();
        }

        private void DefineDelay(ChatMessage message)
        {
            message.Delay
        }

        private string? TreatResponse(ChatMessage message)
        {
            if (message.Content.Contains("[Ne répond pas]"))
            {
                return null;
            }
            return response;
        }
    }
}