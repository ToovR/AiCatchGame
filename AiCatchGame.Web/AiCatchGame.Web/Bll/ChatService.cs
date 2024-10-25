using AiCatchGame.Bo;

namespace AiCatchGame.Web.Bll
{
    public class ChatService : IChatService
    {
        public void PostMessage(ChatMessage message)
        {
            var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
            connection.invoke("SendMessage", user, message);
        }
    }
}