using Discord;
using Discord.WebSocket;
using PorcupineBot.Services;

namespace PorcupineBot.Commands
{
    public delegate Task HandlerCommandMesssage(SocketSlashCommand command);

    public class CommandBuilder
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly Dictionary<string, HandlerCommandMesssage> Commands;
        private readonly List<ApplicationCommandProperties> applicationCommandProperties = new();

        public CommandBuilder(DiscordSocketClient client)
        {
            _discordClient = client;
            _discordClient.Ready += Build;
            Commands = new Dictionary<string, HandlerCommandMesssage>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var derivedTypes = assemblies
                                .SelectMany(assembly => assembly.GetTypes())
                                .Where(t => typeof(BaseCommand).IsAssignableFrom(t) && !t.IsAbstract)
                                .ToList();

            foreach (var derivedType in derivedTypes)
            {
                var instance = Factory.Create<BaseCommand>(derivedType);
                Commands.Add(instance.Name, instance.ExecuteCommand);
                applicationCommandProperties.Add(instance.Build());
            }
        }

        private async Task Build()
        { 
            _discordClient.SlashCommandExecuted += HandlerMessage;  
            await _discordClient.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
        } 

        private async Task HandlerMessage(SocketSlashCommand command)
        {
            await command.DeferAsync();

            if (Commands.ContainsKey(command.CommandName))
            {
                await Commands[command.CommandName].Invoke(command);
            }  
        }
    }
}
