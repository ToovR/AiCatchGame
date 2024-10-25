using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCatchGame.Bo
{
    public class GameClient
    {
        public GameClient(GameStatuses status)
        {
            Status = status;
        }

        public GameStatuses Status { get; }
    }
}