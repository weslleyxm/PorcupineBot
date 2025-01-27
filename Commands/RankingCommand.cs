using Discord;
using Discord.WebSocket;
using PorcupineBot.Repositories;

namespace PorcupineBot.Commands
{
    public class RankingCommand : CommandBase
    {
        private readonly IRankRepository _rankRepository;

        public RankingCommand(IRankRepository database)
        {
            _rankRepository = database;

            WithName("rank");
            WithDescription("shows the ranking of users");
        }

        public override async Task HandlerMessage(SocketSlashCommand command)
        {
            if (command.GuildId == null)
            {
                await command.RespondAsync("Oops, something went wrong");
                return;
            }

            bool exist = await _rankRepository.ExistRank(command.GuildId?.ToString() ?? string.Empty);

            string guildId = command.GuildId.ToString() ?? string.Empty;

            if (!exist)
            {
                await command.RespondAsync("There is no active ranking for this server yet!");
            }
            else
            {
                var rank = await _rankRepository.GetRank(guildId); 
                var votes = await _rankRepository.GetVotes(guildId);

                if (votes.Count <= 0)
                {
                    await command.RespondAsync($"\"{rank?.Name}\" rank has not received any votes yet");
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

                embed.AddField("Rank", rankStr, true);
                embed.AddField("Votos", votesStr, true);
                embed.AddField("Motivo", reasonStr, true);

                embed.WithFooter(rank?.Message);

                await command.RespondAsync(embed: embed.Build());
            }
        }
    }
}