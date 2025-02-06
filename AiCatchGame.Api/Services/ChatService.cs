using AiCatchGame.Bo;
using AiCatchGame.Api.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace AiCatchGame.Api.Services
{
    public class ChatService : IChatService
    {
        public async Task PostMessage(string playerId, string message)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("/GameHub")
                .Build();
            await connection.InvokeAsync("SendMessage", playerId, message);
        }
    }
}