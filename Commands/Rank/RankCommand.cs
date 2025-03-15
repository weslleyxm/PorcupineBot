using Discord.WebSocket;
using PorcupineBot.Repositories;
using PorcupineBot.Services;

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
                await command.FollowupWithLocaleAsync("generic_error");
                return;
            }

            bool exist = await _rankRepository.ExistRank(command.GuildId?.ToString() ?? string.Empty);

            string guildId = command.GuildId.ToString() ?? string.Empty;

            if (!exist)
            {
                await command.FollowupWithLocaleAsync("no_active_ranking");
            }
            else
            {
                var discordClient = ServiceContainer.Resolve<DiscordSocketClient>();

                var rank = await _rankRepository.GetRank(guildId);
                var votes = await _rankRepository.GetVotes(guildId);

                if (votes.Count <= 0)
                {
                    await command.FollowupWithLocaleAsync("no_votes", rank?.Name ?? "");
                    return;
                }

                SocketGuild guild = discordClient.GetGuild(command.GuildId ?? 0);
                string[,] infor = new string[votes.Count(), 3];

                int index = 0;
                foreach (var item in votes)
                {
                    string reason = item.Reason;
                    if (reason.Length > 30)
                        reason = $"{reason.Substring(0, 30)}...";

                    ulong userId = ulong.Parse(item.UserId);

                    var user = guild.GetUserNameAsync(userId);  

                    if (user != null)
                    {
                        infor[index, 0] = $"{user}";
                        infor[index, 1] = $"{item.Votes}";
                        infor[index, 2] = $"{reason}";
                    }

                    index++;
                }

                string serverName = guild.Name;
                string serverIconUrl = guild.IconUrl;

                await ImageGenerator.DownloadIcon(serverIconUrl, serverName);

                ImageGenerator.GenerateRankingImage(serverName, rank?.Name ?? "Ranking without name", infor);

                using (Stream imageStream = ImageGenerator.GetImageStream())
                {
                    await command.FollowupWithFileAsync(imageStream, "discord_ranking.png");
                }
            }
        }
    }
}