using PorcupineBot.Database; 

namespace PorcupineBot.Repositories
{
    public class LocaleRepository : ILocaleRepository
    {
        private DatabaseConnection _databaseConnection;
        public LocaleRepository(DatabaseConnection connection)
        {
            _databaseConnection = connection;
        } 

        public async Task<string> GetLocale(string guildId)
        {
            string locale = string.Empty;
            using (var cmd = _databaseConnection.DbConnection().CreateCommand())
            {
                cmd.CommandText = $"SELECT locale FROM locales WHERE guild_id=@guildId";  
                cmd.Parameters.AddWithValue("@guildId", guildId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        locale = reader.GetString(0);
                    }
                } 
            }

            return locale;
        }
    }
}