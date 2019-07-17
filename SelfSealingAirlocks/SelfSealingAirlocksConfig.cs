using System;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace SelfSealingAirlocks
{
    public class SelfSealingAirlocksConfig
    {
        // Default values
        public bool AirlocksBlockLiquids { get; set; } = false;

        // Load the config file
        public static SelfSealingAirlocksConfig Config;
        public static void LoadConfig()
        {
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Debug.Log("Loading SelfSealingAirlocks config file: " + Path.Combine(modPath, "config.json"));
            Config = LoadConfig<SelfSealingAirlocksConfig>(Path.Combine(modPath, "config.json"));
            Debug.Log("AirlocksBlockLiquids - " + Config.AirlocksBlockLiquids);
        }

        // Config loader
        protected static T LoadConfig<T>(string path)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });

            T result;
            using (StreamReader streamReader = new StreamReader(path))
            {
                using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                {
                    result = jsonSerializer.Deserialize<T>(jsonTextReader);
                    jsonTextReader.Close();
                }
                streamReader.Close();
            }
            return result;
        }
    }
}