using MySql.Data.MySqlClient;
using PorcupineBot.Services;

namespace PorcupineBot.Database
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;
        public DatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;

            ConfigureDatabaseAsync().Wait(); 

            Console.WriteLine("Database configured successfully"); 
        }

        private async Task ConfigureDatabaseAsync()
        {
            using (var cmd = DbConnection().CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ranks (
                                        id INT PRIMARY KEY AUTO_INCREMENT, 
                                        name VARCHAR(255), 
                                        guild_id VARCHAR(255), 
                                        message VARCHAR(255), 
                                        max_votes_at_time INT
                                    )";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS votes (
                                        id INT PRIMARY KEY AUTO_INCREMENT, 
                                        user_id VARCHAR(255), 
                                        votes INT, 
                                        guild_id VARCHAR(255), 
                                        last_reason VARCHAR(255)
                                    )";
                 
                await cmd.ExecuteNonQueryAsync(); 
            }
        }

        public MySqlConnection DbConnection()
        {
            MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
