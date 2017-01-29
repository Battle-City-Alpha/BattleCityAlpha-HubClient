using hub_client.Configuration;
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

        public static AppConfig AppConfig;

        public static GameClient Client { get; private set; }

        #region Windows
        private static Chat _chat;
        #endregion

        public static void Init()
        {
            string AppConfigPath = Path.Combine(AppDataPath, "BattleCityAlphaLauncher", "AppConfig.json");
            if (File.Exists(AppConfigPath))
                AppConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(AppConfigPath));
            else
                AppConfig = new AppConfig();

            Client = new GameClient();
            _chat = new Chat(Client.ChatAdmin);

            StartConnexion();
            _chat.Show();
        }

        public static void StartConnexion()
        {
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
    }
}
