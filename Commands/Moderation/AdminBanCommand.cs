using Discord;
using Discord.WebSocket;

namespace PorcupineBot.Commands.Moderation
{
    /// <summary>
    /// Command to ban a user from the guild
    /// </summary>
    public class AdminBanCommand : BaseCommand
    { 
        public AdminBanCommand()
        {
            WithName("ban");
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
            var daysOption = command.Data.Options.FirstOrDefault(option => option.Name == "days");
            var reasonOption = command.Data.Options.FirstOrDefault(option => option.Name == "reason");

            if (userOption != null)
            {
                var user = (SocketGuildUser)userOption.Value;

                if (user.IsBot) return; 

                if (user.GuildPermissions.ManageGuild)
                {
                    var days = daysOption != null ? (int?)daysOption.Value : null;
                    var reason = reasonOption != null ? (string)reasonOption.Value : string.Empty;

                    await user.BanAsync(days ?? 0, reason); 
                    await command.RespondAsync($"The user {user.Mention} was banned for {days ?? 0} days for the reason of \"{reason}\"");
                }
                else
                {
                    await command.RespondAsync("You do not have permission to ban this user.");
                }
            }
            else
            {
                await command.RespondAsync("Oops, something went wrong");
            }
        }
    }
}
