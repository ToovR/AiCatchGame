using Microsoft.AspNetCore.Components;

namespace AiCatchGame.Web.Client.Components
{
    public partial class ScoreCard
    {
        [Parameter]
        public Bo.GameClient? GameData { get; set; }
    }
}