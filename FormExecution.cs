using BCA.Common;
using BCA.Common.Enums;
using hub_client.Assets;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Helpers;
using hub_client.Network;
using hub_client.Windows;
using hub_client.WindowsAdministrator;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace hub_client
{
    public static class FormExecution
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string HID = HardwareIdManager.GetId();
        public static string Username;

        public static AppConfig AppConfig;
        public static AppDesignConfig AppDesignConfig;
        public static AssetsManager AssetsManager;
        public static ClientConfig ClientConfig;
        public static YgoproConfig YgoproConfig;

        public static GameClient Client { get; private set; }

        public static Dictionary<string, PrivateMessageAdministrator> PrivateForms = new Dictionary<string, PrivateMessageAdministrator>();

        #region Windows
        private static Login _login;
        private static Register _register;
        private static Chat _chat;
        private static Arena _arena;
        private static Shop _shop;
        private static Purchase _purchase;
        private static Brocante _brocante;
        #endregion

        public static void Init()
        {
            string AppConfigPath = Path.Combine(AppDataPath, "BattleCityAlphaLauncher", "AppConfig.json");
            string AppDesignConfigPath = Path.Combine(path, "style.json");
            string ClientConfigPath = Path.Combine(path, "client_config.json");
            AssetsManager = new AssetsManager();
            CardManager.LoadCDB(Path.Combine(path, "BattleCityAlpha", "cards.cdb"), true, true);

            if (File.Exists(AppConfigPath))
                AppConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(AppConfigPath));
            else
                AppConfig = new AppConfig();
            if (File.Exists(AppDesignConfigPath))
                AppDesignConfig = JsonConvert.DeserializeObject<AppDesignConfig>(File.ReadAllText(AppDesignConfigPath));
            else
                AppDesignConfig = new AppDesignConfig();
            if (File.Exists(ClientConfigPath))
                ClientConfig = JsonConvert.DeserializeObject<ClientConfig>(File.ReadAllText(ClientConfigPath));
            else
                ClientConfig = new ClientConfig();

            BoosterManager.LoadList();
            AppDesignConfig = new AppDesignConfig(); //To debug config

            YgoproConfig = new YgoproConfig();

            SaveConfig(AppConfigPath, AppDesignConfigPath);

            Client = new GameClient();

            Client.PopMessageBox += Client_PopMessageBox;
            Client.ChoicePopBox += Client_ChoicePopBox;
            Client.Shutdown += Client_Shutdown;
            Client.PrivateMessageReceived += Client_PrivateMessageReceived;
            Client.LaunchYGOPro += Client_LaunchYGOPro;
            Client.LaunchTrade += Client_LaunchTrade;
            Client.CloseBrocante += Client_CloseBrocante;

            _chat = new Chat(Client.ChatAdmin);
            _login = new Login(Client.LoginAdmin);

            StartConnexion();
            _login.Focus();
            _login.Show();
            logger.Trace("FormExecution initialisation.");
        }

        private static void Client_CloseBrocante()
        {
            logger.Trace("Close Brocante");

            if (_brocante != null && _brocante.IsVisible)
                _brocante.Close();
        }

        private static void Client_LaunchTrade()
        {
            Trade trade = new Trade(Client.TradeAdmin);
            trade.Show();
        }

        private static void Client_ChoicePopBox(PlayerInfo player, RoomConfig config, bool isTrade)
        {
            ChoicePopBox box = new ChoicePopBox(player, config, isTrade);
            box.Owner = _chat;
            box.Topmost = true;
            box.ShowDialog();
        }

        public static void Client_LaunchYGOPro(string arg)
        {
            Process Game = new Process();
            Game.StartInfo.FileName = Path.Combine(FormExecution.path, "BattleCityAlpha", "BCA.exe");
            Game.StartInfo.WorkingDirectory = Path.Combine(FormExecution.path, "BattleCityAlpha");
            Game.StartInfo.Arguments = arg;
            Game.Start();
        }

        private static void Client_PrivateMessageReceived(PlayerInfo user, string message)
        {
            if (PrivateForms.ContainsKey(user.Username))
                PrivateForms[user.Username].PrivateMessageRecieved(message);
            else
            {
                OpenNewPrivateForm(user);
                PrivateForms[user.Username].PrivateMessageRecieved(message);
            }
        }

        public static void OpenNewPrivateForm(PlayerInfo user)
        {
            string username = user.Username;
            PrivateMessageAdministrator admin = new PrivateMessageAdministrator(Client);
            PrivateMessage form = new PrivateMessage(username, admin);
            PrivateForms.Add(username, admin);
            form.Show();
        }

        private static void Client_Shutdown()
        {
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }

        public static bool CanCloseApp()
        {
            if (_chat == null)
                return true;
            return false;
        }

        private static void Client_PopMessageBox(string text, string title, bool showDialog)
        {
            PopBox box = new PopBox(text, title);
            if (showDialog)
                box.ShowDialog();
            else
                box.Show();
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
            Client.Connect(IPAddress.Parse("185.212.226.12"), 9100);
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
        public static void OpenArena()
        {
            logger.Trace("Open arena");
            _arena = new Arena(Client.ArenaAdmin);
            _arena.Show();
        }
        public static void OpenShop()
        {
            logger.Trace("Open Shop");
            _shop = new Shop(Client.ShopAdmin);
            _shop.Show();
        }
        public static void OpenPurchase(string title)
        {
            logger.Trace("Open Purchase");
            _purchase = new Purchase(Client.PurchaseAdmin);
            _purchase.Title = title;
            _purchase.Show();
        }
        public static void OpenTools()
        {
            logger.Trace("Open Tools");
            Tools tools = new Tools(Client.ToolsAdmin);
            tools.Show();
        }
        public static void OpenBrocante()
        {
            logger.Trace("Open Brocante");

            if (_brocante != null && _brocante.IsVisible)
                _brocante.Activate();

            _brocante = new Brocante(Client.BrocanteAdmin);
            _brocante.Show();
        }
        public static void OpenDuelRequest(int id)
        {
            logger.Trace("Open Duel Request Form");

            DuelRequest request = new DuelRequest(Client.DuelRequestAdmin, id);
            request.Show();
        }
    }
}
