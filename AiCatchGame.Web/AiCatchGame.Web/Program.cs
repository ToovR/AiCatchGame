using AiCatchGame.Web.Api;
using AiCatchGame.Web.Components;
using AiCatchGame.Web.HostedServices;
using AiCatchGame.Web.Interfaces;
using AiCatchGame.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// https://www.c-sharpcorner.com/article/building-a-real-time-chat-application-with-net-core-7-signalr/

builder.Services.AddSignalR();
builder.Services.AddMudServices();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddHostedService<GameHostedService>();
var app = builder.Build();

app.MapHub<GameHub>("/gameHub");

app.MapGroup("api/Game").MapGame();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(AiCatchGame.Web.Client._Imports).Assembly);

app.Run();