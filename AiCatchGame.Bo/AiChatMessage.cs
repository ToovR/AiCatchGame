using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCatchGame.Bo
{
    public class AiChatMessage
    {
        public double Delay { get; }
        public Guid PlayerId { get; }
        public string Content { get; }

        public AiChatMessage(Guid playerId, string content, double delay)
        {
            PlayerId = playerId;
            Content = content;
            Delay = delay;

        }

    }
}
