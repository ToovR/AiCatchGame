using AiCatchGame.Api.Interfaces;
using AiCatchGame.Bo;
using System.Text.RegularExpressions;

namespace AiCatchGame.Api.Services
{
    public class AiPlayerService : IAiPlayerService
    {
        private List<AiPlayer> _aiPlayers = [];
        private IChatService _chatService;
        private DateTime _lastMessageTime;
        private IAiLllmClient _llmClient;
        private List<CTAChatMessage> _messages = [];
        private List<AiChatMessage> _messagesToSend = [];
        private Queue<string> _responses = [];
        //private Dictionary<Guid, AiPlayerClientService> clientServices = [];

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

        public AiPlayerService()
        {
            _llmClient = new MistralLocalllmClient();
            _llmClient.SetSystemMessage(defaultSystemMessage);
        }

        public async Task InitializeAi()
        {
            await _llmClient.Initialize();
        }

        public IEnumerable<AiChatMessage> ManageResponse()
        {
            for (int i = 0; i < _messagesToSend.Count; i++)
            {
                AiChatMessage messageToSend = _messagesToSend[i];
                double timeToWait = messageToSend.Delay - (DateTime.Now - _lastMessageTime).TotalSeconds;
                timeToWait = timeToWait >= 0 ? timeToWait : 0;
                if (timeToWait == 0)
                {
                    _messagesToSend.RemoveAt(i);
                    i--;
                    yield return messageToSend;
                }
            }
        }

        public async Task OnPlayerSpeak(CTAChatMessage message, AiPlayer player)
        {
            if (player.Character.Id == message.CharacterId)
            {
                return;
            }
            _lastMessageTime = message.Time;
            _messages.Add(message);
            // TODO Add to AI LLM received message and get response
            string response = await TreatInAi(player, message);

            Console.WriteLine($"==> Réponse : {response}");
            if (response.Contains("[Ne répond pas]"))
            {
                return;
            }

            response = response.Replace("Assistant:", "")
                .Replace("User:", "")
                .Replace("System:", "")
                .Trim();
            Match m = Regex.Match(response, "\\[TEMPS:(\\d+)s\\] ");
            double delay = DefineDelay(message);
            if (m.Success)
            {
                if (m.Groups.Count > 1 && m.Groups[1].Success && double.TryParse(m.Groups[1].Value, out double res))
                {
                    delay = res;
                }

                response = response.Substring(0, m.Index) + response.Substring(m.Index + m.Length);
            }
            AiChatMessage aiChatMessage = new AiChatMessage(player.PrivateId, player.Character!.Id, response, delay);
            _messagesToSend.Add(aiChatMessage);
        }

        private double DefineDelay(CTAChatMessage message)
        {
            // TODO Define delay while considering other players delay
            return ((double)new Random().Next(500, 2000)) / 1000;
        }

        private async Task<string> TreatInAi(AiPlayer player, CTAChatMessage message)
        {
            string response = await _llmClient.GenerateText($"{player.Character.Name} a dit : {message.Content}");

            //string response = new Random().Next(0, 2) == 0 ? "Message Test" : "[Ne répond pas]";
            return response;
        }
    }
}