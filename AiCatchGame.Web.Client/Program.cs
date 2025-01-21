using AiCatchGame.Web.Client.Interfaces;
using AiCatchGame.Web.Client.Services;
using AiCatchGame.Web.Shared;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.AddSharedServices();
builder.Services.AddScoped<INetClient, NetClient>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();