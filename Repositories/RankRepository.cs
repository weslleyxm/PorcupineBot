using PorcupineBot.Database;
using PorcupineBot.Models;

namespace PorcupineBot.Repositories
{
    public class RankRepository : IRankRepository
    {
        private DatabaseConnection _databaseConnection;
        public RankRepository(DatabaseConnection connection)
        {
            _databaseConnection = connection;
        }

        public async Task CreateRank(string name, string guildId, string message, int maxVotesAtTime)
        {
            using (var cmd = _databaseConnection.DbConnection().CreateCommand())
            {
                cmd.CommandText = $"INSERT INTO ranks (name, guild_id, message, max_votes_at_time) VALUES(@name, @guildId, @message, @maxVotesAtTime)";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@guildId", guildId);
                cmd.Parameters.AddWithValue("@message", message);
                cmd.Parameters.AddWithValue("@maxVotesAtTime", maxVotesAtTime); 
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public Task DeleteRank(string guildId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistRank(string guildId)
        {
            using (var cmd = _databaseConnection.DbConnection().CreateCommand())
            {
                cmd.CommandText = $"SELECT COUNT(*) FROM ranks WHERE guild_id=@guildId";
                cmd.Parameters.AddWithValue("@guildId", guildId);
                int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                return count > 0;
            }
        }

        public async Task InsertVote(string userId, int votes, string guildId, string reason)
        {
            string commandText = "INSERT INTO votes (user_id, votes, guild_id, last_reason) VALUES  (@userId, @votes, @guildId, @reason)";
            if (await ExistVote(userId))
            {
                commandText = "UPDATE votes SET votes = votes + @votes, guild_id = @guildId, last_reason = @reason WHERE user_id = @userId LIMIT 1"; 
            }

            using (var cmd = _databaseConnection.DbConnection().CreateCommand()) 
            {
                cmd.CommandText = commandText;
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@votes", votes);
                cmd.Parameters.AddWithValue("@guildId", guildId);
                cmd.Parameters.AddWithValue("@reason", reason);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> ExistVote(string userId)
        {
            using (var cmd = _databaseConnection.DbConnection().CreateCommand())
            {
                cmd.CommandText = $"SELECT COUNT(*) FROM votes WHERE user_id=@userId";
                cmd.Parameters.AddWithValue("@userId", userId);
                int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                return count > 0;
            }
        }

        public async Task<List<VoteModel>> GetVotes(string guildId)
        {
            var votesList = new List<VoteModel>();
            using (var cmd = _databaseConnection.DbConnection().CreateCommand())
            {
                cmd.CommandText = $"SELECT votes, user_id, last_reason FROM votes WHERE guild_id=@guildId ORDER BY votes DESC";
                cmd.Parameters.AddWithValue("@guildId", guildId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        votesList.Add(new VoteModel
                        {
                            Votes = reader.GetInt32(0),
                            UserId = reader.GetString(1),
                            Reason = reader.GetString(2)
                        });
                    }
                }
            }

            return votesList;
        }

        public async Task<RankModel?> GetRank(string guildId) 
        {
            RankModel? rankModel = null;
            using (var cmd = _databaseConnection.DbConnection().CreateCommand())
            {
                cmd.CommandText = $"SELECT name, message, max_votes_at_time FROM ranks WHERE guild_id=@guildId"; 
                cmd.Parameters.AddWithValue("@guildId", guildId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        rankModel = new RankModel
                        {
                            Name = reader.GetString(0),
                            Message = reader.GetString(1),
                            MaxVotesAtTime = reader.GetInt32(2)
                        };
                    }
                }
            }

            return rankModel;
        }
    }
}
