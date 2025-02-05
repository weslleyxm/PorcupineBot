using Discord;
using Discord.WebSocket;
using PorcupineBot.Repositories;

namespace PorcupineBot.Commands.Rank
{
    public class RankCommand : BaseCommand
    {
        private readonly IRankRepository _rankRepository;

        public RankCommand(IRankRepository database)
        {
            _rankRepository = database;

            WithName("rank");
            WithDescription("shows the ranking of users");
        }

        public override async Task ExecuteCommand(SocketSlashCommand command)
        {
            if (command.GuildId == null)
            {
                await command.FollowupAsync("Oops, something went wrong");
                return;
            }
             
            bool exist = await _rankRepository.ExistRank(command.GuildId?.ToString() ?? string.Empty);

            string guildId = command.GuildId.ToString() ?? string.Empty;

            if (!exist)
            {
                await command.FollowupAsync("There is no active ranking for this server yet!");
            }
            else
            {
                var rank = await _rankRepository.GetRank(guildId);
                var votes = await _rankRepository.GetVotes(guildId);

                if (votes.Count <= 0)
                {
                    await command.FollowupAsync($"\"{rank?.Name}\" rank has not received any votes yet");
                    return;
                }

                var embed = new EmbedBuilder
                {
                    Title = rank?.Name,
                    Color = Color.Blue
                };

                string rankStr = string.Empty;
                string votesStr = string.Empty;
                string reasonStr = string.Empty;

                int count = 1;
                foreach (var item in votes)
                {
                    rankStr += $"#{count} ︱ <@{item.UserId}>\n";
                    votesStr += $"{item.Votes}\n";
                    reasonStr += $"{item.Reason}\n";
                    count++;
                }

                embed.AddField("Rank", string.IsNullOrWhiteSpace(rankStr) ? "nothing" : rankStr, true);
                embed.AddField("Votos", string.IsNullOrWhiteSpace(votesStr) ? "0" : votesStr, true);
                embed.AddField("Motivo", string.IsNullOrWhiteSpace(reasonStr) ? "nothing" : reasonStr, true);
                 
                embed.WithFooter(rank?.Message);

                await command.FollowupAsync(embed: embed.Build()); 
            }
        }
    }
}