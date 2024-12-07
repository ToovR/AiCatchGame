using AiCatchGame.Web.Components;
using AiCatchGame.Web.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AiCatchGame.Web;
using MudBlazor.Services;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();


 // https://www.c-sharpcorner.com/article/building-a-real-time-chat-application-with-net-core-7-signalr/

builder.Services.AddSignalR();
builder.Services.AddMudServices();
builder.Services.AddControllersWithViews();




var app = builder.Build();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{Game}/{action=Index}/{id?}");
app.MapHub<GameHub>("/gameHub");


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


