using AiCatchGame.Web.Client.Interfaces;
using AiCatchGame.Web.Client.Services;
using AiCatchGame.Web.Shared;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

string? apiUrl = builder.Configuration.GetValue<string>("ApiUrl");

#if DEBUG
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7262") });
#else
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://aicatchgameapi.azurewebsites.net/") });
#endif
builder.Services.AddMudServices();

builder.Services.AddSharedServices();
builder.Services.AddScoped<INetClient, NetClient>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();