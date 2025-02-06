using AiCatchGame.Api.Interfaces;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace AiCatchGame.Api.Services
{
    public class AzureAiLlmClient : IAiLllmClient
    {
        private const string openAi_deploymentOrModelName = "CatchTheAi";
        private const string openAi_endpoint = "https://catchtheaiopenai.openai.azure.com/";
        private const string openAi_key = "PUT KEY Here";
        private ChatClient _chatClient;
        private string _systemMessage;

        public async Task<string> GenerateText(string prompt)
        {
            if (_chatClient == null)
            {
                await Initialize(_systemMessage);
            }

            var messages = new List<OpenAI.Chat.ChatMessage>();
            messages.Add(new UserChatMessage(prompt));
            var response = await _chatClient.CompleteChatAsync(messages, new ChatCompletionOptions()
            {
                Temperature = (float)0.7,
                FrequencyPenalty = (float)0,
                PresencePenalty = (float)0,
            });
            string chatResponse = response.Value.Content.Last().Text;

            return chatResponse;
        }

        public async Task Initialize()
        {
            await Initialize(_systemMessage);
        }

        public async Task Initialize(string defaultSystemMessage)
        {
            var llmClient = new AzureOpenAIClient(new Uri(openAi_endpoint), new ApiKeyCredential(openAi_key));
            _chatClient = llmClient.GetChatClient(openAi_deploymentOrModelName);
            var messages = new List<OpenAI.Chat.ChatMessage>();
            messages.Add(new SystemChatMessage(defaultSystemMessage));
            var response = await _chatClient.CompleteChatAsync(messages, new ChatCompletionOptions()
            {
                Temperature = (float)0.7,
                FrequencyPenalty = (float)0,
                PresencePenalty = (float)0,
            });
            var chatResponse = response.Value.Content.Last().Text;
        }

        public void SetSystemMessage(string defaultSystemMessage)
        {
            _systemMessage = defaultSystemMessage;
        }
    }
}