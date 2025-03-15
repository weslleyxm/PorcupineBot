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

        public static async Task<string> GetUserNameAsync(ulong userId)
        {
            if (userNamesCache.TryGetValue(userId, out var cachedName))
            {
                return cachedName;
            }

            var user = await discordClient.GetUserAsync(userId);

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
