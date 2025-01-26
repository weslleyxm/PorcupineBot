using Discord;
using Discord.WebSocket;

namespace PorcupineBot.Commands
{
    public abstract class CommandBase : SlashCommandBuilder
    {
        public abstract Task HandlerMessage(SocketSlashCommand command);
    }
}
