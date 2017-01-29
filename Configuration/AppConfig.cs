using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Configuration
{
    public class AppConfig
    {
        public bool RememberMe = false;
        public string Username;
        public string Password;
        public string Key = AppTools.GenerateKey();

        public void Save()
        {
            if (!Directory.Exists(Path.Combine(FormExecution.AppDataPath, "BattleCityAlphaLauncher")))
                Directory.CreateDirectory(Path.Combine(FormExecution.AppDataPath, "BattleCityAlphaLauncher"));
            File.WriteAllText(Path.Combine(FormExecution.AppDataPath, "BattleCityAlphaLauncher", "AppConfig.json"), JsonConvert.SerializeObject(this));
        }
    }
}
