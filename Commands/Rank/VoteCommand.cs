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
                       .WithName("user")
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
            if (command.User.Id != 297868291878158352)
            {
                await command.FollowupWithLocaleAsync("not_kenzy");
                return;
            }

            var userOption = command.Data.Options.FirstOrDefault(option => option.Name == "user");
            var reasonOption = command.Data.Options.FirstOrDefault(option => option.Name == "reason");
            var pointsOption = command.Data.Options.FirstOrDefault(option => option.Name == "points");

            if (command.GuildId == null)
            {
                await command.FollowupWithLocaleAsync("generic_error");
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
                        await command.FollowupWithLocaleAsync("too_many_votes", rank?.MaxVotesAtTime ?? 0);
                        return;
                    }

                    await _rankRepository.InsertVote(user.Id.ToString(), votes, guildId, reason);

                    await command.FollowupWithLocaleAsync("vote_registered", user.Mention, votes, reason);
                }
                else
                {
                    await command.FollowupWithLocaleAsync("invalid_command");
                }
            }
            else
            {
                await command.FollowupWithLocaleAsync("no_rank");
            }
        }
    }
}