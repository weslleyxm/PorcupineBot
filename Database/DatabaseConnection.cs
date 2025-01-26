using System.Data.SQLite;

namespace PorcupineBot.Database
{
    public class DatabaseConnection
    {
        private static SQLiteConnection? sqliteConnection;
        private readonly string _path;
        public DatabaseConnection() 
        {
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.sqlite");

            if (!File.Exists(_path))
            {
                SQLiteConnection.CreateFile(_path); 
            }
      
            using (var cmd = DbConnection().CreateCommand())
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS ranks(id INTEGER PRIMARY KEY AUTOINCREMENT, name Varchar(255), guild_id Varchar(255), message Varchar(255), max_votes_at_time INTEGER)";
                cmd.ExecuteNonQuery(); 
            } 

            using (var cmd = DbConnection().CreateCommand()) 
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS votes(id INTEGER PRIMARY KEY AUTOINCREMENT, user_id Varchar(255), votes INTEGER, guild_id Varchar(255), last_reason Varchar(255))";
                cmd.ExecuteNonQuery(); 
            }

            Console.WriteLine("Database configured successfully");
        }

        public SQLiteConnection DbConnection() 
        {
            sqliteConnection = new SQLiteConnection($"Data Source={_path}; Version=3;");
            sqliteConnection.Open();
            return sqliteConnection;
        }
    }
}
