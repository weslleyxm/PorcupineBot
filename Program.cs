using PorcupineBot;
using PorcupineBot.Database;
using PorcupineBot.Repositories;
using PorcupineBot.Services;

ServiceContainer.AddSingleton<DatabaseConnection>();  
ServiceContainer.AddScoped<IRankRepository, RankRepository>();  

var app = BotClient.Build();
await app.Run();