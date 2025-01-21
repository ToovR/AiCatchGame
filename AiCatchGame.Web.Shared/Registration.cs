using AiCatchGame.Web.Shared.Interfaces;
using AiCatchGame.Web.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AiCatchGame.Web.Shared
{
    public static class Registration
    {
        public static void AddSharedServices(this IServiceCollection services)
        {
            services.AddSingleton<IHubClientService, HubClientService>();
        }
    }
}