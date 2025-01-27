using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PorcupineBot.Commands;
using PorcupineBot.Services;

namespace PorcupineBot.Extensions
{
    public static class AppExtensions
    {
        public static void AddCommands(this DiscordSocketClient discord)
        {
            var discordClient = ServiceContainer.Resolve<DiscordSocketClient>();
            var instance = new CommandBuilder(discordClient);

            ServiceContainer.ConfigureServices(services =>
            {
                services.AddSingleton(instance);
            }); 
        }
    }
}
