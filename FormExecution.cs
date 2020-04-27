﻿using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Assets;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Helpers;
using hub_client.Network;
using hub_client.Stuff;
using hub_client.Windows;
using hub_client.WindowsAdministrator;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace hub_client
{
    public static class FormExecution
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static string debug_ip = "127.0.0.1";
        //public static string debug_ip = "185.212.226.12";
        public static string release_ip = "185.212.226.12";

        public static string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string HID = HardwareIdManager.GetId();
        public static string Username;


        public static string AppConfigPath = Path.Combine(AppDataPath, "BattleCityAlphaLauncher", "AppConfig.json");
        public static string AppDesignConfigPath = Path.Combine(path, "style.json");
        public static string ClientConfigPath = Path.Combine(path, "client_config.json");
        public static string YgoProConfigPath = Path.Combine(path, "ygopro_config.json");
        public static AppConfig AppConfig;
        public static AppDesignConfig AppDesignConfig;
        public static AssetsManager AssetsManager;
        public static ClientConfig ClientConfig;

        public static GameClient Client { get; private set; }

        public static Dictionary<int, PrivateMessageAdministrator> PrivateForms = new Dictionary<int, PrivateMessageAdministrator>();

        #region Windows
        private static UpdateCardsStuffWindow _windowload;
        private static Login _login;
        private static Register _register;
        private static Chat _chat;
        private static Arena _arena;
        private static Shop _shop;
        private static Purchase _purchase;
        private static Brocante _brocante;
        private static PrestigeShop _pshop;
        private static Tools _tools;
        #endregion

        public static string GetIp()
        {
#if DEBUG
            return debug_ip;
#else
            return release_ip;
#endif
        }

        public static void Init()
        {
            AssetsManager = new AssetsManager();

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
            //AppDesignConfig = new AppDesignConfig(); //To debug config
            
            SaveConfig();

            _windowload = new UpdateCardsStuffWindow(new string[] { }, true);
            _windowload.Show();

            CardManager.LoadingFinished += CardManager_LoadingFinished;
            CardManager.LoadingProgress += CardManager_LoadingProgress;
            Task.Run(() => CardManager.LoadCDB(Path.Combine(path, "BattleCityAlpha", "cards.cdb"), true, true));


            Client = new GameClient();

            Client.PopMessageBox += Client_PopMessageBox;
            Client.ChoicePopBox += Client_ChoicePopBox;
            Client.RoomNeedPassword += Client_RoomNeedPassword;
            Client.Shutdown += Client_Shutdown;
            Client.PrivateMessageReceived += Client_PrivateMessageReceived;
            Client.LaunchYGOPro += Client_LaunchYGOPro;
            Client.LaunchYGOProWithoutRoom += Client_LaunchYGOProWithoutRoom;
            Client.LoadPlayerCustomizations += Client_LoadPlayerCustomizations;
            Client.LaunchTrade += Client_LaunchTrade;
            Client.CloseBrocante += Client_CloseBrocante;
            Client.LaunchBonusBox += Client_LaunchBonusBox;
            Client.LaunchDuelResultBox += Client_LaunchDuelResultBox;
            Client.LoadOfflineMessages += Client_LoadOfflineMessages;
            Client.RecieveDeck += Client_RecieveDeck;

            _chat = new Chat(Client.ChatAdmin);
            _login = new Login(Client.LoginAdmin);

            logger.Trace("FormExecution initialisation.");
        }

        private static void Client_RecieveDeck(PlayerInfo sender, string[] decklist, string deckname)
        {
            File.WriteAllLines(Path.Combine(path, "BattleCityAlpha", "deck", sender.Username + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".ydk"), decklist);
            Client_PopMessageBox("Vous avez reçu le deck " + deckname + " de la part de " + sender.Username + ".", "Partage de deck", true);
            YgoProHelper.LaunchYgoPro("-d " + sender.Username + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss"));
        }

        private static void CardManager_LoadingProgress(int i, int total)
        {
            double progress = ((double)i / (double)total) * 100;
            _windowload.SetProgressValue(progress);
        }

        private static void CardManager_LoadingFinished()
        {
            logger.Trace("CDB Loaded.");
            _windowload.EndDownload();
            _windowload.Close();

            StartConnexion();
            _login.Focus();
            _login.Show();

            CardManager.LoadingProgress -= CardManager_LoadingProgress;
            CardManager.LoadingFinished -= CardManager_LoadingFinished;
        }

        public static void HideLogin()
        {
            _login.Hide();
        }
        public static void ShowLogin()
        {
            _login.Show();
        }

        private static void Client_RoomNeedPassword(int id, RoomType type)
        {
            InputText form = new InputText();
            form.Title = "Mot de passe";
            form.Owner = _arena;
            form.SelectedText += (obj) => RoomPassInput_SelectedText(obj, id, type);
            form.ShowDialog();
        }
        private static void RoomPassInput_SelectedText(string pass, int id, RoomType type)
        {
            Client.SendRoomNeedPassword(id, pass, type);
        }

        private static void Client_LoadPlayerCustomizations(Customization avatar, Customization border, Customization sleeve, int pos)
        {
            YgoProHelper.LoadCustomization(avatar, border, sleeve, pos);
        }

        private static void Client_LaunchYGOProWithoutRoom(string arg)
        {
            YgoProHelper.LaunchYgoPro(arg);
        }

        private static void Client_LoadOfflineMessages(OfflineMessage[] messages)
        {
            OfflineMessagesBox box = new OfflineMessagesBox();
            box.Owner = _chat;
            box.LoadMessages(messages);
            box.ShowDialog();
        }

        private static void Client_LaunchDuelResultBox(int bps, int exps, bool win)
        {
            DuelResult box = new DuelResult(bps, exps, win);
            box.Owner = _chat;
            box.Topmost = true;
            box.Show();
        }

        private static void Client_LaunchBonusBox(BonusType type, int numberconnexion, string gift, int[] cards)
        {
            BonusBox box = new BonusBox(type, numberconnexion, gift);
            box.Owner = _chat;
            box.ShowDialog();
            if (type == BonusType.Booster)
            {
                OpenPurchase(new BoosterInfo { Name = gift, Type = PurchaseType.Booster });
                _purchase.UpdateCards(cards);
            }
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
            trade.Owner = _chat;
            trade.Show();
        }

        private static void Client_ChoicePopBox(PlayerInfo player, RoomConfig config, bool isTrade, string pass)
        {
            ChoicePopBox box = new ChoicePopBox(player, config, isTrade, pass);
            box.Owner = _chat;
            box.Topmost = true;
            box.ShowDialog();
        }

        public static void Client_LaunchYGOPro(Room room, string arg)
        {
            if (room.IsRanked() && _arena != null && _arena.RankedTimer.IsEnabled)
                _arena.StopTimer();
            YgoProHelper.LaunchGame(room, arg);
        }

        private static void Client_PrivateMessageReceived(PlayerInfo user, string message)
        {
            if (PrivateForms.ContainsKey(user.UserId))
                PrivateForms[user.UserId].PrivateMessageRecieved(user, message);
            else
            {
                if (Client.BlacklistManager.CheckBlacklist(user))
                    return;
                OpenNewPrivateForm(user);
                PrivateForms[user.UserId].PrivateMessageRecieved(user, message);
            }
        }

        public static void OpenNewPrivateForm(PlayerInfo user)
        {
            if (PrivateForms.ContainsKey(user.UserId))
                return;

            string username = user.Username;
            PrivateMessageAdministrator admin = new PrivateMessageAdministrator(Client);
            PrivateMessage form = new PrivateMessage(username, admin);
            PrivateForms.Add(user.UserId, admin);
            form.Owner = _chat;
            form.Show();
            form.Closed += (sender,e) => PMClosed(sender, e, user.UserId);
        }

        private static void PMClosed(object sender, EventArgs e, int userID)
        {
            PrivateForms.Remove(userID);
        }

        public static Chat GetChatWindow()
        {
            return _chat;
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

        public static void Client_PopMessageBox(string text, string title, bool showDialog)
        {
            PopBox box = new PopBox(text, title);
            box.Topmost = true;
            if (showDialog)
                box.ShowDialog();
            else
                box.Show();
        }

        public static void SaveConfig()
        {
            if (!Directory.Exists(AppConfigPath))
                Directory.CreateDirectory(Path.Combine(AppDataPath, "BattleCityAlphaLauncher"));
            File.WriteAllText(AppConfigPath, JsonConvert.SerializeObject(AppConfig));

            File.WriteAllText(AppDesignConfigPath, JsonConvert.SerializeObject(AppDesignConfig, Formatting.Indented));

            File.WriteAllText(ClientConfigPath, JsonConvert.SerializeObject(ClientConfig, Formatting.Indented));

            logger.Trace("Config Saved.");
        }

        public static void StartConnexion()
        {
            logger.Trace("Attempt of connexion...");

            Client.Connect(IPAddress.Parse(GetIp()), 9100);
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
            //_register.Owner = _login;
            _register.Topmost = true;
            _register.Show();
        }
        public static void OpenArena()
        {
            logger.Trace("Open arena");
            _arena = new Arena(Client.ArenaAdmin);
            _arena.Owner = _chat;
            _arena.Show();
        }
        public static void OpenShop()
        {
            logger.Trace("Open Shop");
            _shop = new Shop(Client.ShopAdmin);
            _shop.Owner = _chat;
            _shop.Show();
        }
        public static void OpenPrestigeShop()
        {
            logger.Trace("Open Prestige Shop");
            _pshop = new PrestigeShop(Client.PrestigeShopAdmin);
            Client.Send(PacketType.OpenPrestigeShop, new StandardClientOpenPrestigeShop { });
            _pshop.Owner = _shop;
            _pshop.Show();
        }
        public static void OpenPrestigeCustomizationsViewer()
        {
            logger.Trace("Open Prestige Customizations viewer");
            PrestigeCustomizationsViewerHorizontal viewer = new PrestigeCustomizationsViewerHorizontal(Client.PrestigeCustomizationsViewerAdmin, true);
            viewer.Owner = _pshop;
            viewer.Topmost = true;
            viewer.Show();
        }
        public static void OpenPrestigeCustomizationsVerticalViewer()
        {
            logger.Trace("Open Prestige Customizations vertical viewer");
            PrestigeCustomizationViewerVertical viewer = new PrestigeCustomizationViewerVertical(Client.PrestigeCustomizationsViewerAdmin, true);
            viewer.Owner = _pshop;
            viewer.Show();
        }
        public static void OpenPrestigeTitleViewer()
        {
            TitlesHandle form = new TitlesHandle(Client.TitlesHandleAdmin, true);
            form.Owner = _pshop;
            form.Show();
        }
        public static void OpenPurchase(BoosterInfo booster)
        {
            logger.Trace("Open Purchase");
            _purchase = new Purchase(Client.PurchaseAdmin, booster);
            _purchase.Title = booster.Name;
            _purchase.Owner = _shop;
            _purchase.Show();
        }
        public static void OpenTools()
        {
            logger.Trace("Open Tools");
            _tools = new Tools(Client.ToolsAdmin);
            _tools.Owner = _chat;
            _tools.Show();
        }
        public static void OpenBrocante()
        {
            logger.Trace("Open Brocante");

            if (_brocante != null && _brocante.IsVisible)
                _brocante.Activate();

            _brocante = new Brocante(Client.BrocanteAdmin);
            _brocante.Owner = _shop;
            _brocante.Show();
        }
        public static void OpenDuelRequest(int id)
        {
            logger.Trace("Open Duel Request Form");

            DuelRequest request = new DuelRequest(Client.DuelRequestAdmin, id);
            request.Topmost = true;
            request.Show();
        }
        public static void OpenSoloModeWindow()
        {
            logger.Trace("Open Solo mode");

            SoloMode sm = new SoloMode();
            sm.Owner = _arena;
            sm.Show();
        }
        public static void OpenDatasRetrievalWindow()
        {
            DataRetrievalWindow window = new DataRetrievalWindow(Client.DataRetrievalAdmin);
            window.Owner = _tools;
            window.ShowDialog();
        }
        public static void OpenChangePicsWindow()
        {
            ChangePicsStyle window = new ChangePicsStyle();
            window.Owner = _tools;
            window.ShowDialog();
        }
        public static void OpenRankingWindow()
        {
            RankingWindow window = new RankingWindow(Client.RankingDisplayAdmin);
        }

        public static void RefreshChatStyle()
        {
            _chat.LoadStyle();
        }
    }
}
