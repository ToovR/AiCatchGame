using AiCatchGame.Bo;
using AiCatchGame.Web.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace AiCatchGame.Web.Bll
{
    public class ChatService : IChatService
    {
        public async Task PostMessage(Guid playerId, string message)
        {

            var connection = new HubConnectionBuilder()
                .WithUrl("/GameHub")
                .Build();
            await connection.InvokeAsync("SendMessage", playerId, message);
        }
    }
}