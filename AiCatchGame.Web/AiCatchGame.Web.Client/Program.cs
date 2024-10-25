using AiCatchGame.Web.Client.Interfaces;
using AiCatchGame.Web.Client.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

// https://github.com/Blazored/LocalStorage
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddHttpClient();
builder.Services.AddScoped<INetClient, NetClient>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();