using Discord;
using Discord.WebSocket;

namespace PorcupineBot.Commands
{
    public abstract class BaseCommand : SlashCommandBuilder
    {
        public abstract Task ExecuteCommand(SocketSlashCommand command);
    }
}
