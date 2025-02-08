using Discord.Rest;
using Discord.WebSocket;
using PorcupineBot.Translation;

public static class LocalizationExtensions
{
    public static async Task<RestFollowupMessage> FollowupWithLocaleAsync(this SocketSlashCommand command, string key, params object[] @params)
    {
        string text = await Localization._(key, command.GuildId, @params);
        return await command.FollowupAsync(text);
    }
}