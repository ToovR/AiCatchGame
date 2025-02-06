using AiCatchGame.Api.Controllers;
using AiCatchGame.Api.Interfaces;
using AiCatchGame.Api.Services;
using AiCatchGame.Web.HostedServices;
using AiCatchGame.Web.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IAiPlayerService, AiPlayerService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddHostedService<GameHostedService>();
builder.Services.AddSharedServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.MapHub<GameHub>("/gameHub");

app.MapGroup("api/Game").MapGame();

app.Run();