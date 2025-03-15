using Discord;
using Discord.WebSocket;

namespace PorcupineBot.Services
{
    public static class UserCache
    {
        private static readonly Dictionary<ulong, string> userNamesCache = new Dictionary<ulong, string>();
        private static readonly IDiscordClient discordClient;

        static UserCache()
        {
            discordClient = ServiceContainer.Resolve<DiscordSocketClient>();
        }

        public static string GetUserNameAsync(this SocketGuild guild, ulong userId)
        {
            if (userNamesCache.TryGetValue(userId, out var cachedName))
            {
                return cachedName;
            } 

            var user = guild.GetUser(userId);

            if (user != null)
            {
                string userName = user.Username;
                userNamesCache[userId] = userName;
                return userName;
            }

            return string.Empty;
        }
    }

}
