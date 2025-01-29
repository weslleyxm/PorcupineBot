using Discord;
using Discord.WebSocket;
using PorcupineBot.Repositories;

namespace PorcupineBot.Commands.Rank
{
    public class NewRankCommand : BaseCommand
    {
        private readonly IRankRepository _rankRepository;
        public NewRankCommand(IRankRepository repository)
        {
            _rankRepository = repository;

            WithName("new-rank");
            WithDescription("create a new rank for your server");
            AddOptions(new SlashCommandOptionBuilder()
                       .WithName("name")
                       .WithType(ApplicationCommandOptionType.String)
                       .WithDescription("rank name")
                       .WithRequired(true),

                        new SlashCommandOptionBuilder()
                       .WithName("message")
                       .WithType(ApplicationCommandOptionType.String)
                       .WithDescription("This field is intended for the first of the rank")
                       .WithRequired(true),

                        new SlashCommandOptionBuilder()
                       .WithName("max-votes")
                       .WithType(ApplicationCommandOptionType.Integer)
                       .WithDescription("The maximum amount of votes at a time")
                       .WithRequired(true));
        }

        public override async Task ExecuteCommand(SocketSlashCommand command)
        {
            var nameOption = command.Data.Options.FirstOrDefault(option => option.Name == "name");
            var messageOption = command.Data.Options.FirstOrDefault(option => option.Name == "message");
            var maxVotesAtTimeOption = command.Data.Options.FirstOrDefault(option => option.Name == "max-votes");

            if (command.GuildId == null || nameOption == null || messageOption == null || maxVotesAtTimeOption == null)
            {
                await command.RespondAsync("Oops, something went wrong");
                return;
            }

            bool exist = await _rankRepository.ExistRank(command.GuildId?.ToString() ?? string.Empty);

            if (exist)
            {
                await command.RespondAsync("There is already an active rank for this server!");
            }
            else
            {
                string name = (string)nameOption.Value;
                string message = (string)messageOption.Value;
                string guildId = command.GuildId.ToString() ?? string.Empty;
                int maxVotesAtTime = Convert.ToInt32(maxVotesAtTimeOption.Value);
                await _rankRepository.CreateRank(name, guildId, message, maxVotesAtTime);
                await command.RespondAsync($"Rank \"{name}\" was successfully created the maximum points per vote allowed is {maxVotesAtTime}");
            }
        }
    }
}
