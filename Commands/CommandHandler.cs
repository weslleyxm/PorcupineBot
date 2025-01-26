using Discord;
using Discord.WebSocket;
using PorcupineBot.Services;

namespace PorcupineBot.Commands
{
    public delegate Task HandlerCommandMesssage(SocketSlashCommand command);

    public class CommandHandler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly Dictionary<string, HandlerCommandMesssage> Commands;
        private readonly List<ApplicationCommandProperties> applicationCommandProperties = new();

        public CommandHandler(DiscordSocketClient client)
        {
            _discordClient = client;
            _discordClient.Ready += CreateCommands;
            Commands = new Dictionary<string, HandlerCommandMesssage>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var derivedTypes = assemblies
                                .SelectMany(assembly => assembly.GetTypes())
                                .Where(t => typeof(CommandBase).IsAssignableFrom(t) && !t.IsAbstract)
                                .ToList();

            foreach (var derivedType in derivedTypes)
            {
                var instance = Factory.Create<CommandBase>(derivedType);
                Commands.Add(instance.Name, instance.HandlerMessage);
                applicationCommandProperties.Add(instance.Build());
            }
        }

        private async Task CreateCommands()
        {
            _discordClient.SlashCommandExecuted += HandlerMessage;
            ulong guildId = 1019467451597078559;
            await _discordClient.Rest.BulkOverwriteGuildCommands(applicationCommandProperties.ToArray(), guildId);
        }

        private async Task HandlerMessage(SocketSlashCommand command)
        {
            if (Commands.ContainsKey(command.CommandName))
            {
                await Commands[command.CommandName].Invoke(command);
            }
        }
    }
}
