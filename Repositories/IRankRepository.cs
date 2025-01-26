using PorcupineBot.Models;

namespace PorcupineBot.Repositories
{
    public interface IRankRepository
    {
        Task<bool> ExistRank(string guildId);
        Task CreateRank(string name, string guildId, string message, int maxVotesAtTime);
        Task DeleteRank(string guildId);
        Task InsertVote(string userId, int votes, string guildId, string reason);
        Task<bool> ExistVote(string userId);
        Task<List<VoteModel>> GetVotes(string guildId);
        Task<RankModel?> GetRank(string guildId);
    }
}
