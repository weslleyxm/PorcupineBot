using Newtonsoft.Json.Linq;

namespace PorcupineBot.Services
{ 
    public static class Appsettings
    {
        private static JObject? keys;
        public static void LoadAppsettings()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            using (StreamReader r = new StreamReader(path)) 
            {
                string json = r.ReadToEnd();
                JObject data = JObject.Parse(json);
                if (data != null)
                {
                    keys = data;
                }
                else
                {
                    throw new InvalidOperationException("Failed to load configuration from JSON file");
                }
            } 
        }

        public static string GetString(string key)
        {
            if (keys == null)
            {
                throw new InvalidOperationException("Configuration has not been loaded. Call 'LoadAppsettings' first");
            }

            return (keys[key]?.ToString() ?? throw new KeyNotFoundException($"The key '{key}' was not found")); 
        }
    }
}
