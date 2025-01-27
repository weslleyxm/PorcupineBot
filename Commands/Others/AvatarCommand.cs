using Discord;
using Discord.WebSocket;

namespace PorcupineBot.Commands.Others
{
    public class AvatarCommand : BaseCommand
    {
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
         
        public async override Task ExecuteCommand(SocketSlashCommand command)
        {
            var userOption = command.Data.Options.FirstOrDefault(option => option.Name == "user");

            if (userOption == null)
            {
                await command.RespondAsync("Oops, something went wrong");
                return; 
            }

            var user = (SocketGuildUser)userOption.Value; 
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Auto, 1024) ?? user.GetDefaultAvatarUrl();
             
            var embed = new EmbedBuilder();
            embed.WithTitle($":camera_with_flash: {user.Username}")
                .WithFooter("The guy's image")
                .WithColor(Color.Blue)
                .WithImageUrl(avatarUrl);  

            await command.RespondAsync(embed: embed.Build());
        }
    }
}
