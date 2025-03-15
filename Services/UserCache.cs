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

        public static async Task<string> GetUserNameAsync(this SocketGuild _guild, ulong userId)
        {
            if (userNamesCache.TryGetValue(userId, out var cachedName))
            {
                return cachedName;
            } 

            foreach (var guild in await discordClient.GetGuildsAsync()) 
            {
                if (guild.Id == _guild.Id)
                {
                    var user = await guild.GetUserAsync(userId); 

                    if (user != null)
                    {
                        string userName = user.Nickname ?? user.GlobalName ?? user.DisplayName;
                        string formattedUsername = userName.Length > 17 ? $"{userName.Substring(0, 17)}..." : userName;
                        userNamesCache[userId] = formattedUsername;
                        return formattedUsername; 
                    } 
                }
            }
              
            return string.Empty;
        }
    }

}
