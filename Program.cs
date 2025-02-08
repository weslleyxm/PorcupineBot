using PorcupineBot;
using PorcupineBot.Database;
using PorcupineBot.Repositories;
using PorcupineBot.Services;

Appsettings.LoadAppsettings();
 
string connectionString = Appsettings.GetString("connectionString") ?? string.Empty;

ServiceContainer.AddSingleton<DatabaseConnection>(new DatabaseConnection(connectionString)); 
ServiceContainer.AddScoped<IRankRepository, RankRepository>();
ServiceContainer.AddScoped<ILocaleRepository, LocaleRepository>();

var app = BotClient.Build();
await app.Run();