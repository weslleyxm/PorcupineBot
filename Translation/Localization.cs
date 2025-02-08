using Newtonsoft.Json;
using PorcupineBot.Repositories;
using PorcupineBot.Services;
using System.Text;

namespace PorcupineBot.Translation
{
    public static class Localization
    {
        private static Dictionary<string, Dictionary<string, string>> Locales = new();
        private static Dictionary<string, string> ServerLocales = new();
        private static ILocaleRepository LocaleRepository;

        static Localization()
        {
            LocaleRepository = ServiceContainer.Resolve<ILocaleRepository>(); 
        }

        private static string GetPath(string locale)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Locales/{locale}.json");
            return path;
        }

        public static async Task<string> _(string key, ulong? guildId, params object[] @params)
        { 
            string guildIdString = guildId.ToString() ?? ""; 
             
            if (!ServerLocales.TryGetValue(guildIdString, out string? localizedKey))
            {
                localizedKey = await LocaleRepository.GetLocale(guildIdString);
                   
                if (string.IsNullOrEmpty(localizedKey))
                {
                    localizedKey = "en-US";
                }

                ServerLocales[guildIdString] = localizedKey;
            }

            if (!Locales.TryGetValue(localizedKey, out var translations))
            {
                string jsonPath = GetPath(localizedKey);

                if (File.Exists(jsonPath))
                {
                    try
                    {
                        string json = File.ReadAllText(jsonPath, Encoding.UTF8);
                        var newLocale = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        if (newLocale != null)
                        {
                            Locales[localizedKey] = newLocale;
                            translations = newLocale;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading locale {localizedKey}: {ex.Message}"); ;
                    }
                }
                else
                {
                    Console.WriteLine($"The translation file does not exist");
                }
            } 

            string text = translations?.TryGetValue(key, out var value) == true ? value : key;
            return string.Format(text, @params);
        } 
    }
}