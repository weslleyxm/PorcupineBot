using Discord;
using Discord.WebSocket;

namespace PorcupineBot.Commands.Others
{
    /// <summary>
    /// Command to show a user's avatar
    /// </summary>
    public class AvatarCommand : BaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvatarCommand"/> 
        /// </summary>
        public AvatarCommand()
        {
            WithName("avatar");
            WithDescription("show user avatar");
            AddOptions(new SlashCommandOptionBuilder()
                       .WithName("user")
                       .WithType(ApplicationCommandOptionType.User)
                       .WithDescription("the users you want to see the avatar")
                       .WithRequired(true));
        }

        /// <summary>
        /// Executes the avatar command
        /// </summary>
        /// <param name="command">The command to execute</param>
        public async override Task ExecuteCommand(SocketSlashCommand command)
        {
            var userOption = command.Data.Options.FirstOrDefault(option => option.Name == "user");

            if (userOption == null)
            {
                await command.FollowupAsync("Oops, something went wrong");
                return;
            }

            var user = (SocketGuildUser)userOption.Value;
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Auto, 1024) ?? user.GetDefaultAvatarUrl();

            var embed = new EmbedBuilder();
            embed.WithTitle($":camera_with_flash: {user.Username}")
                .WithFooter("The guy's image")
                .WithColor(Color.Blue)
                .WithImageUrl(avatarUrl);

            await command.FollowupAsync(embed: embed.Build());
        }
    }
}
