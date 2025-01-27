using Discord.WebSocket;
using Discord;
using PorcupineBot.Extensions;
using PorcupineBot.Services;
using PorcupineBot.Database;

namespace PorcupineBot
{
    public class BotClient
    {
        private readonly DiscordSocketClient _socketClient; 

        private readonly string _token = "";  

        public BotClient(DiscordSocketClient client)
        {
            Appsettings.LoadAppsettings();

            _token = Appsettings.GetString("token") ?? string.Empty;

            Console.WriteLine(_token);

            _socketClient = client; 
        } 

        public static BotClient Build()
        {
            ServiceContainer.AddSingleton<DiscordSocketClient>();
           
            var discord = Factory.Create<BotClient>(typeof(BotClient));
            return discord;
        }

        public async Task Run()
        {
            await _socketClient.LoginAsync(TokenType.Bot, _token);
            await _socketClient.StartAsync();

            _socketClient.AddCommands();   

            _socketClient.Log += Log; 

            await _socketClient.SetGameAsync("Just relaxing waiting for a command");
            await Task.Delay(-1);
        } 

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
