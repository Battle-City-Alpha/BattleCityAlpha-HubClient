using Newtonsoft.Json;
using System.IO;

namespace hub_client.Configuration
{
    public class AppConfig
    {
        public bool RememberMe = false;
        public string Username;
        public string Password;

        public void Save()
        {
            if (!Directory.Exists(Path.Combine(FormExecution.AppDataPath, "BattleCityAlphaLauncher")))
                Directory.CreateDirectory(Path.Combine(FormExecution.AppDataPath, "BattleCityAlphaLauncher"));
            File.WriteAllText(Path.Combine(FormExecution.AppDataPath, "BattleCityAlphaLauncher", "AppConfig.json"), JsonConvert.SerializeObject(this));
        }
    }
}
