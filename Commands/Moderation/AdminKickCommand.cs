using Discord;
using Discord.WebSocket;

namespace PorcupineBot.Commands.Moderation
{
    /// <summary>
    /// Command to kick a user from the guild
    /// </summary>
    public class AdminKickCommand : BaseCommand
    {
        public AdminKickCommand()
        {
            WithName("kick");
            WithDescription("ban a specific user");
            AddOptions(new SlashCommandOptionBuilder()
                       .WithName("user")
                       .WithType(ApplicationCommandOptionType.User)
                       .WithDescription("user you want to ban")
                       .WithRequired(true),

                       new SlashCommandOptionBuilder()
                       .WithName("days")
                       .WithType(ApplicationCommandOptionType.Integer)
                       .WithDescription("for how many days")
                       .WithRequired(false),

                       new SlashCommandOptionBuilder()
                       .WithName("reason")
                       .WithType(ApplicationCommandOptionType.String)
                       .WithDescription("what is the reason")
                       .WithRequired(false));
        }

        public async override Task ExecuteCommand(SocketSlashCommand command)
        {
            var userOption = command.Data.Options.FirstOrDefault(option => option.Name == "user");
            var reasonOption = command.Data.Options.FirstOrDefault(option => option.Name == "reason");

            if (userOption != null)
            {
                var user = (SocketGuildUser)userOption.Value;

                if (user.IsBot) return;

                if (user.GuildPermissions.ManageGuild)
                {
                    var reason = reasonOption != null ? (string)reasonOption.Value : string.Empty;
                    await user.KickAsync(reason);
                    await command.FollowupAsync($"The user {user.Mention} was kicked for the reason of \"{reason}\""); ;
                }
                else
                {
                    await command.FollowupAsync("You do not have permission to ban this user.");
                }
            }
            else
            {
                await command.FollowupAsync("Oops, something went wrong");
            }
        }
    }
}
