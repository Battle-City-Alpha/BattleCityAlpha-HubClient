using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Configuration;
using hub_client.Helpers;
using hub_client.Network;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        ChatAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        private ChatCommandParser _cmdParser;

        private List<PlayerInfo> Players;

        public Chat(ChatAdministrator admin)
        {
            InitializeComponent();

            _admin = admin;
            _cmdParser = new ChatCommandParser();

            _admin.ChatMessage += _admin_ChatMessage;
            this.Loaded += Chat_Loaded;
            _admin.LoginComplete += _admin_LoginComplete;
            _admin.AddHubPlayer += _admin_AddHubPlayer;
            _admin.RemoveHubPlayer += _admin_RemoveHubPlayer;
            _admin.ClearChat += _admin_ClearChat;

            Players = new List<PlayerInfo>();
            lvUserlist.Items.Clear();
            lvUserlist.ItemsSource = Players;

            tbUserlist.tbChat.TextChanged += TbChat_TextChanged;
        }

        private void TbChat_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO Find player.
        }

        private void UpdateList()
        {
            try
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUserlist.ItemsSource);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("Rank");
                view.GroupDescriptions.Clear();
                view.GroupDescriptions.Add(groupDescription);
            }
            catch (Exception ex) { }
        }

        private void _admin_ClearChat(string username, string reason)
        {
            chat.Clear();
            _admin_ChatMessage(style.InformationMessageColor, String.Format("Le chat a été nettoyé par {0}. Raison : {1}.", username, reason), true, true);
        }

        private void _admin_RemoveHubPlayer(PlayerInfo infos)
        {
            Players.Remove(infos);
            UpdateList();
            if (FormExecution.ClientConfig.Connexion_Message)
                _admin_ChatMessage(style.LauncherMessageColor, String.Format("{0} s'est déconnecté.", infos.Username), false, false);
            logger.Trace("{0} added to userlist.", infos);
        }
        private void _admin_AddHubPlayer(PlayerInfo infos)
        {
            Players.Add(infos);
            UpdateList();
            if (FormExecution.ClientConfig.Connexion_Message)
                _admin_ChatMessage(style.LauncherMessageColor, String.Format("{0} s'est connecté.", infos.Username), false, false);
            logger.Trace("{0} removed from userlist.", infos);
        }

        private void _admin_LoginComplete()
        {
            Show();
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
            logger.Trace("Style loaded.");
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> HeadButtons = new List<BCA_ColorButton>();
            HeadButtons.AddRange(new[] { btnArene, btnShop, btnDecks, btnAnimations, btnTools });
            List<BCA_ColorButton> BottomButtons = new List<BCA_ColorButton>();
            BottomButtons.AddRange(new[] { btnProfil, btnFAQ, btnReplay, btnNote, btnDiscord });

            foreach(BCA_ColorButton btn in HeadButtons)
            {
                btn.Color1 = style.Color1HomeHeadButton;
                btn.Color2 = style.Color2HomeHeadButton;
                btn.Update();
            }
            foreach (BCA_ColorButton btn in BottomButtons)
            {
                btn.Color1 = style.Color1HomeBottomButton;
                btn.Color2 = style.Color2HomeBottomButton;
                btn.Update();
            }
        }

        private void _admin_ChatMessage(Color c, string msg, bool italic, bool bold)
        {
            Dispatcher.InvokeAsync(delegate { chat.OnColoredMessage(c, msg, italic, bold); });
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnDiscord.ClickedAnimation();
            System.Diagnostics.Process.Start("https://discordapp.com/invite/seEZAwV");
            logger.Trace("Discord clicked.");
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            btnDiscord.ReleasedAnimation();
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            btnNote.ClickedAnimation();
            Notes note = new Notes(_admin.Client.NotesAdmin);
            note.Show();
            logger.Trace("Notes clicked.");
        }

        private void Image_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            btnNote.ReleasedAnimation();
        }

        private void btnFAQ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forum.battlecityalpha.xyz/thread-681.html");
            logger.Trace("FAQ clicked.");
        }

        private void btnCGU_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.Client_LaunchYGOPro("-r");
        }

        private void tbChat_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    NetworkData data = ParseMessage(tbChat.GetText());
                    if (data != null && data.Packet != null)
                    {
                        _admin.Client.Send(data);
                        logger.Info("Chat send : {0}.", data);
                    }
                    tbChat.Clear();
                    break;
            }
            e.Handled = true;
        }
        private NetworkData ParseMessage(string txt)
        {
            try
            {
                if (txt[0] == '/')
                {
                    txt = txt.Substring(1);
                    string cmd = txt.Split(' ')[0].ToString().ToUpper();
                    switch (cmd)
                    {
                        case "ANIM":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.AnimationMessage(txt.Substring(cmd.Length + 1)));
                        case "INFO":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.InformationMessage(txt.Substring(cmd.Length + 1)));
                        case "SETMOTD":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.SetMessageOfTheDay(txt.Substring(cmd.Length + 1)));
                        case "SETGREET":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.SetGreet(txt.Substring(cmd.Length + 1)));
                        case "KICK":
                            return new NetworkData(PacketType.Kick, _cmdParser.Kick(txt.Substring(cmd.Length + 1)));
                        case "BAN":
                            return new NetworkData(PacketType.Ban, _cmdParser.Ban(txt.Substring(cmd.Length + 1)));
                        case "MUTE":
                            return new NetworkData(PacketType.Mute, _cmdParser.Mute(txt.Substring(cmd.Length + 1)));
                        case "CLEAR":
                            return new NetworkData(PacketType.Clear, _cmdParser.ClearChat(txt.Substring(cmd.Length + 1)));
                        case "MPALL":
                            return new NetworkData(PacketType.MPAll, _cmdParser.MPAll(txt.Substring(cmd.Length + 1)));
                        case "PANEL":
                            Panel panel = new Panel(_admin.Client.PanelAdmin);
                            panel.Show();
                            return null;
                        case "BANLIST":
                            return new NetworkData(PacketType.Banlist, new StandardClientBanlist { });
                        case "GIVEBATTLEPOINTS":
                            return new NetworkData(PacketType.GivePoints, _cmdParser.GiveBattlePoints(txt.Substring(cmd.Length + 1)));
                        case "GIVEPRESTIGEPOINTS":
                            return new NetworkData(PacketType.GivePoints, _cmdParser.GivePrestigePoints(txt.Substring(cmd.Length + 1)));
                        case "GIVECARD":
                            return new NetworkData(PacketType.GiveCard, _cmdParser.GiveCard(txt.Substring(cmd.Length + 1)));
                        case "GIVEAVATAR":
                            return new NetworkData(PacketType.GiveAvatar, _cmdParser.GiveAvatar(txt.Substring(cmd.Length + 1)));
                        case "ENABLED":
                            return new NetworkData(PacketType.EnabledAccount, _cmdParser.EnabledAccount(txt.Substring(cmd.Length + 1)));
                        case "DISABLED":
                            return new NetworkData(PacketType.DisabledAccount, _cmdParser.DisabledAccount(txt.Substring(cmd.Length + 1)));
                        case "PROMOTE":
                            return new NetworkData(PacketType.Ranker, _cmdParser.Ranker(txt.Substring(cmd.Length + 1)));
                        case "BLACKLIST":
                            Blacklist blacklist = new Blacklist(_admin.Client.BlacklistManager);
                            blacklist.Show();
                            return null;
                        default:
                            _admin_ChatMessage(FormExecution.AppDesignConfig.LauncherMessageColor, "••• Cette commande n'existe pas.", false, false);
                            return null;
                    }
                }
                return new NetworkData(PacketType.ChatMessage, _cmdParser.StandardMessage(txt));
            }
            catch (Exception ex)
            {
                _admin_ChatMessage(FormExecution.AppDesignConfig.LauncherMessageColor, "••• Une erreur s'est produite.", false, false);
                logger.Error("Chat input error : {0}", ex.ToString());
                return null;
            }
        }

        private void lbUserlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PlayerInfo target = FormExecution.Client.GetPlayerInfo(lvUserlist.SelectedItem.ToString()); 
            if (target != null)
                FormExecution.OpenNewPrivateForm(target);
        }

        private void btnProfil_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Profil profil = new Profil(_admin.Client.ProfilAdmin);
            profil.Show();
            NetworkData data = new NetworkData(PacketType.Profil, new StandardClientProfilAsk { Username = _admin.Client.GetPlayerInfo(FormExecution.Username) });
            _admin.Client.Send(data);
        }

        private void btnDecks_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NetworkData data = new NetworkData(PacketType.UpdateCollection, new StandardClientAskCollection());
            _admin.Client.Send(data);
        }

        private void btnArene_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.OpenArena();
            //_admin.Client.OpenPopBox("Fonctionnalité à venir !", "Message technique");
        }

        private void btnShop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.OpenShop();
        }

        private void btnAnimations_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forum.battlecityalpha.xyz/forum-25.html");
            logger.Trace("Animations clicked.");
        }

        private void btnTools_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.Client.Send(PacketType.LoadAvatar, new StandardClientLoadAvatars());
            FormExecution.OpenTools();
        }

        private void pm_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = FormExecution.Client.GetPlayerInfo(lvUserlist.SelectedItem.ToString());
            if (target != null)
                FormExecution.OpenNewPrivateForm(target);
        }

        private void duelrequest_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            string target = lvUserlist.SelectedItem.ToString();
            //if (target != null)
            // TODO DuelRequest
        }

        private void traderequest_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = ((PlayerInfo)lvUserlist.SelectedItem);
            if (target != null)
                    FormExecution.Client.Send(PacketType.TradeRequest, new StandardClientTradeRequest { Player = target });
        }

        private void setblacklist_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = ((PlayerInfo)lvUserlist.SelectedItem);
            if (target != null)
            {
                _admin.Client.BlacklistManager.AddPlayer(target);
                _admin.Client.BlacklistManager.AddPlayer(target);
                _admin.Client.BlacklistManager.Save();
                _admin_ChatMessage(FormExecution.AppDesignConfig.LauncherMessageColor, String.Format("••• Vous avez ajouté à votre blacklist : {0}.", target.Username), false, false);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.ChatMessage -= _admin_ChatMessage;
            Loaded -= Chat_Loaded;
            _admin.LoginComplete -= _admin_LoginComplete;
            _admin.AddHubPlayer -= _admin_AddHubPlayer;
            _admin.RemoveHubPlayer -= _admin_RemoveHubPlayer;
            _admin.ClearChat -= _admin_ClearChat;
            tbUserlist.tbChat.TextChanged -= TbChat_TextChanged;

            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }
    }
}
