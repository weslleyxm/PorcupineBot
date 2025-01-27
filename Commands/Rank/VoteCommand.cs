using Discord;
using Discord.WebSocket;
using PorcupineBot.Repositories;

namespace PorcupineBot.Commands.Rank
{
    public class VoteCommand : BaseCommand
    {
        private readonly IRankRepository _rankRepository;

        public VoteCommand(IRankRepository database)
        {
            _rankRepository = database;

            WithName("vote");
            WithDescription("Vote for someone to receive points");
            AddOptions(new SlashCommandOptionBuilder()
                       .WithName("name")
                       .WithType(ApplicationCommandOptionType.User)
                       .WithDescription("Who the vote goes to")
                       .WithRequired(true),

                       new SlashCommandOptionBuilder()
                       .WithName("points")
                       .WithType(ApplicationCommandOptionType.Integer)
                       .WithDescription("how many points?")
                       .WithRequired(true),

                        new SlashCommandOptionBuilder()
                       .WithName("reason")
                       .WithType(ApplicationCommandOptionType.String)
                       .WithDescription("why is he gaining these points?")
                       .WithRequired(true));
        }

        public override async Task ExecuteCommand(SocketSlashCommand command)
        {
            var userOption = command.Data.Options.FirstOrDefault(option => option.Name == "name");
            var pointsOption = command.Data.Options.FirstOrDefault(option => option.Name == "points");
            var reasonOption = command.Data.Options.FirstOrDefault(option => option.Name == "reason");

            if (command.GuildId == null)
            {
                await command.RespondAsync("Oops, something went wrong");
                return;
            }

            bool exist = await _rankRepository.ExistRank(command.GuildId?.ToString() ?? string.Empty);

            if (exist)
            {
                if (userOption != null && pointsOption != null && reasonOption != null)
                {
                    var user = (SocketGuildUser)userOption.Value;
                    var votes = Convert.ToInt32(pointsOption.Value);
                    var reason = (string)reasonOption.Value;

                    var guildId = command.GuildId.ToString() ?? string.Empty;
                    var rank = await _rankRepository.GetRank(guildId);

                    if (votes > rank?.MaxVotesAtTime)
                    {
                        await command.RespondAsync($"What are you trying to do??? You can only have {rank?.MaxVotesAtTime} votes at a time");
                        return;
                    }

                    await _rankRepository.InsertVote(user.Id.ToString(), votes, guildId, reason);

                    await command.RespondAsync($"You voted for {user.Mention} to receive {votes} points, for the reason that \"{reason}\"");
                }
                else
                {
                    await command.RespondAsync("Invalid command options");
                }
            }
            else
            {
                await command.RespondAsync("Too bad, it seems no rank has been created");
            }
        }
    }
}