using hub_client.Configuration;
using hub_client.Helpers;
using hub_client.Network;
using hub_client.Windows;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hub_client
{
    public static class FormExecution
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string HID = HardwareIdManager.GetId();

        public static AppConfig AppConfig;
        public static AppDesignConfig AppDesignConfig;

        public static GameClient Client { get; private set; }

        #region Windows
        private static Login _login;
        private static Register _register;
        private static Chat _chat;
        #endregion

        public static void Init()
        {
            string AppConfigPath = Path.Combine(AppDataPath, "BattleCityAlphaLauncher", "AppConfig.json");
            string AppDesignConfigPath = Path.Combine(path, "style.json");

            if (File.Exists(AppConfigPath))
                AppConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(AppConfigPath));
            else
                AppConfig = new AppConfig();
            if (File.Exists(AppDesignConfigPath))
                AppDesignConfig = JsonConvert.DeserializeObject<AppDesignConfig>(File.ReadAllText(AppDesignConfigPath));
            else
                AppDesignConfig = new AppDesignConfig();


            AppDesignConfig = new AppDesignConfig(); //To debug config

            SaveConfig(AppConfigPath, AppDesignConfigPath);

            Client = new GameClient();

            Client.PopMessageBox += Client_PopMessageBox;

            _chat = new Chat(Client.ChatAdmin);
            _login = new Login(Client.LoginAdmin);

            StartConnexion();
            _login.Show();
            logger.Trace("FormExecution initialisation.");
        }

        public static bool CanCloseApp()
        {
            if (_chat == null)
                return true;
            return false;
        }

        private static void Client_PopMessageBox(string text, string title)
        {
            PopBox box = new PopBox(text, title);
            box.ShowDialog();
        }

        public static void SaveConfig(string ConfigPath, string DesignConfigPath)
        {
            if (!Directory.Exists(ConfigPath))
                Directory.CreateDirectory(Path.Combine(AppDataPath, "BattleCityAlphaLauncher"));
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(AppConfig));
            File.WriteAllText(DesignConfigPath, JsonConvert.SerializeObject(AppDesignConfig, Formatting.Indented));
            logger.Trace("Config Saved.");
        }

        public static void StartConnexion()
        {
            logger.Trace("Attempt of connexion...");
#if DEBUG
            Client.Connect(IPAddress.Parse("127.0.0.1"), 9100);
#else
            Client.Connect(IPAddress.Parse("46.105.240.229"), 9100);
#endif
            Client.Connected += Client_Connected;
            Task.Run(() => UpdateNetwork());
        }

        private static void Client_Connected()
        {
            logger.Trace("GameClient connected.");
        }

        private static void UpdateNetwork()
        {
            Client.Update();
            Task.Delay(1).ContinueWith((previous) => UpdateNetwork());
        }

        public static void OpenRegisterForm()
        {
            logger.Trace("Open register form");
            _register = new Register(Client.RegisterAdmin);
            _register.Show();
        }
    }
}
