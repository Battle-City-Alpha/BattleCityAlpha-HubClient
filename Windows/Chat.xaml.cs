﻿using BCA.Common;
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
using System.Linq;
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

        private List<PlayerInfo> Players;
        private List<PlayerInfo> PlayersFound;

        public Chat(ChatAdministrator admin)
        {
            InitializeComponent();

            _admin = admin;

            _admin.ChatMessage += _admin_ChatMessage;
            this.Loaded += Chat_Loaded;
            _admin.LoginComplete += _admin_LoginComplete;
            _admin.AddHubPlayer += _admin_AddHubPlayer;
            _admin.RemoveHubPlayer += _admin_RemoveHubPlayer;
            _admin.ClearChat += _admin_ClearChat;

            tbUserList.TextChanged += SearchUser;

            Players = new List<PlayerInfo>();
            PlayersFound = new List<PlayerInfo>();
            lvUserlist.ItemsSource = Players;

        }

        private void SearchUser(object sender, TextChangedEventArgs e)
        {
            PlayersFound.Clear();
            if (tbUserList.Text != "")
            {
                foreach (PlayerInfo info in Players)
                    if (info.Username.ToLower().Contains(tbUserList.Text.ToLower()))
                        PlayersFound.Add(info);

                lvUserlist.ItemsSource = PlayersFound;
            }
            else
                lvUserlist.ItemsSource = Players;

            lvUserlist.Items.Refresh();
        }

        private void _admin_ClearChat(string username, string reason)
        {
            chat.Clear();
            _admin_ChatMessage(style.InformationMessageColor, String.Format("Le chat a été nettoyé par {0}. Raison : {1}.", username, reason), true, true);
        }

        private void _admin_RemoveHubPlayer(PlayerInfo infos)
        {
            Players.Remove(infos);
            lvUserlist.Items.Refresh();

            if (FormExecution.ClientConfig.Connexion_Message)
                _admin_ChatMessage(style.LauncherMessageColor, String.Format("{0} s'est déconnecté.", infos.Username), false, false);
            logger.Trace("{0} added to userlist.", infos);
        }
        private void _admin_AddHubPlayer(PlayerInfo infos)
        {
            AddPlayer(infos);

            if (FormExecution.ClientConfig.Connexion_Message)
                _admin_ChatMessage(style.LauncherMessageColor, String.Format("{0} s'est connecté.", infos.Username), false, false);
            logger.Trace("{0} added to userlist.", infos);
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
                    _admin.SendMessage(tbChat.GetText());
                    tbChat.Clear();
                    break;
            }
            e.Handled = true;
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
            _admin.SendProfileAsking();
        }

        private void btnDecks_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.SendDeck();
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
                _admin.SendTradeRequest(target);
        }

        private void setblacklist_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = ((PlayerInfo)lvUserlist.SelectedItem);
            if (target != null)
            {
                _admin.AddBlacklistPlayer(target);
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
            tbUserList.TextChanged -= SearchUser;

            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }

        private void AddPlayer(PlayerInfo infos)
        {
                for (int i = 0; i < Players.Count; i++)
                    if (Players[i].Rank <= infos.Rank)
                    {
                        Players.Insert(i, infos);
                        break;
                    }
            if (!Players.Contains(infos))
                Players.Add(infos);

            lvUserlist.Items.Refresh();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUserlist.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Rank");
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(groupDescription);
        }
    }
}
