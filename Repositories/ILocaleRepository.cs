using PorcupineBot.Models;

namespace PorcupineBot.Repositories
{
    public interface ILocaleRepository
    {
        Task<string> GetLocale(string guildId);
    }
}
