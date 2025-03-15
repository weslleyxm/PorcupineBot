using Discord.WebSocket;
using Discord;
using PorcupineBot.Extensions;
using PorcupineBot.Services;

namespace PorcupineBot
{
    /// <summary>
    /// Bot client for the PorcupineBot application
    /// </summary>
    public class BotClient
    {
        private readonly DiscordSocketClient _socketClient;
        private readonly string _token = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="BotClient"/> class
        /// </summary>
        /// <param name="client">The Discord socket client</param>
        public BotClient(DiscordSocketClient client)
        {
            _token = Appsettings.GetString("token") ?? string.Empty;
            _socketClient = client;
        }

        /// <summary>
        /// Builds a new instance of the <see cref="BotClient"/> class
        /// </summary>
        /// <returns>A new instance of the <see cref="BotClient"/> class</returns>
        public static BotClient Build()
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMembers |
                             GatewayIntents.GuildMessages
            };

            ServiceContainer.AddSingleton<DiscordSocketClient>(new DiscordSocketClient(config));

            var discord = Factory.Create<BotClient>(typeof(BotClient));
            return discord;
        }

        /// <summary>
        /// Runs the bot client
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task Run()
        {
            await _socketClient.LoginAsync(TokenType.Bot, _token);
            await _socketClient.StartAsync();

            _socketClient.AddCommands();

            _socketClient.Log += Log;

            await _socketClient.SetCustomStatusAsync("Simplemente disfrutando el momento");
            await Task.Delay(-1);
        }

        /// <summary>
        /// Logs the specified message
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <returns>A completed task</returns>
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
