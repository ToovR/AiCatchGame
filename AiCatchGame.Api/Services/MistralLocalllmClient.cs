using AiCatchGame.Api.Interfaces;
using LLama;
using LLama.Common;
using System.Text;

namespace AiCatchGame.Api.Services
{
    public class MistralLocalllmClient : IAiLllmClient
    {
        private readonly string _modelPath = @"C:\Users\jb\.cache\lm-studio\models\TheBloke\OpenHermes-2.5-Mistral-7B-GGUF\openhermes-2.5-mistral-7b.Q4_K_M.gguf";
        private ChatSession _session;
        private string _systemMessage;

        public async Task DisposeSession()
        {
        }

        public async Task<string> GenerateText(string prompt)
        {
            if (_session == null)
            {
                await Initialize(_systemMessage);
            }
            InferenceParams inferenceParams = new InferenceParams()
            {
                MaxTokens = 1024,
                AntiPrompts = new List<string> { "User:" },
            };
            StringBuilder text = new StringBuilder();
            await foreach (var textPart in _session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), inferenceParams))
            {
                text.Append(textPart);
            }
            return text.ToString();
        }

        public async Task Initialize()
        {
            if (_session == null)
            {
                await Initialize(_systemMessage);
                await GenerateText("C'est parti");
            }
        }

        public void SetSystemMessage(string systemMessage)
        {
            _systemMessage = systemMessage;
        }

        private async Task Initialize(string defaultSystemMessage)
        {
            ModelParams parameters = new ModelParams(_modelPath)
            {
                ContextSize = 2048,
                Seed = 1337,
                GpuLayerCount = 4
            };

            LLamaWeights model = LLamaWeights.LoadFromFile(parameters);
            LLamaContext context = model.CreateContext(parameters);
            InteractiveExecutor executor = new InteractiveExecutor(context);

            ChatHistory chatHistory = new ChatHistory();
            chatHistory.AddMessage(AuthorRole.System, "Tu es un assistant qui parle en Français");
            chatHistory.AddMessage(AuthorRole.System, defaultSystemMessage);
            _session = new ChatSession(executor, chatHistory);
        }
    }
}