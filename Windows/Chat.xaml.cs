﻿using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
using hub_client.Assets;
using hub_client.Configuration;
using hub_client.Helpers;
using hub_client.Stuff;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        [DllImport("user32")] public static extern int FlashWindow(IntPtr hwnd, bool bInvert);

        private static Logger logger = LogManager.GetCurrentClassLogger();

        ChatAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        public List<PlayerItem> Players;
        private List<PlayerItem> PlayersFound;

        InputText form = new InputText();
        AssetsManager PicsManager = new AssetsManager();

        private List<string> _last_messages;
        int _index_last_message = 0;

        public Chat(ChatAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _admin = admin;

            _admin.SpecialChatMessage += _admin_SpecialChatMessage;
            _admin.PlayerChatMessage += _admin_PlayerChatMessage;
            this.Loaded += Chat_Loaded;
            _admin.LoginComplete += _admin_LoginComplete;
            _admin.AddHubPlayer += _admin_AddHubPlayer;
            _admin.RemoveHubPlayer += _admin_RemoveHubPlayer;
            _admin.UpdateHubPlayers += _admin_UpdateHubPlayers;
            _admin.ClearChat += _admin_ClearChat;

            tbUserList.TextChanged += SearchUser;
            lvUserlist.MouseDoubleClick += LvUserlist_MouseDoubleClick;

            tbChat.PreviewKeyDown += TbChat_PreviewKeyDown;

            Players = new List<PlayerItem>();
            PlayersFound = new List<PlayerItem>();
            lvUserlist.ItemsSource = Players;

            _last_messages = new List<string>();

            this.MouseDown += Chat_MouseDown;

            this.Title = "Battle City Alpha - " + Main.VERSION;

        }

        public void Flash()
        {
            WindowInteropHelper wih = new WindowInteropHelper(FormExecution.GetChat());
            FlashWindow(wih.Handle, true);
        }

        private void TbChat_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    string[] words = tbChat.GetText().Split(' ');
                    words[words.Length - 1] = FindUsername(words[words.Length - 1]);
                    tbChat.SetText(String.Join(" ", words));
                    tbChat.tbChat.SelectionStart = tbChat.tbChat.Text.Length;
                    tbChat.tbChat.SelectionLength = 0;
                    e.Handled = true;
                    break;
            }
        }

        private void LvUserlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = lvUserlist.SelectedItem as PlayerInfo;
            if (target != null)
                FormExecution.OpenNewPrivateForm(target);
        }

        private void _admin_UpdateHubPlayers(PlayerInfo[] players, PlayerState state)
        {
            foreach (PlayerInfo player in players)
                if (player != null)
                    foreach (PlayerItem item in lvUserlist.Items)
                        if (item.UserId == player.UserId)
                            item.State = state;

            lvUserlist.Items.Refresh();
        }

        private void _admin_PlayerChatMessage(Color c, PlayerInfo p, string msg)
        {
            Dispatcher.InvokeAsync(delegate { chat.OnPlayerColoredMessage(c, p, msg); });
        }

        private void _admin_SpecialChatMessage(Color c, string msg, bool italic, bool bold)
        {
            Dispatcher.InvokeAsync(delegate { chat.OnSpecialColoredMessage(c, msg, italic, bold); });
        }

        private void SearchUser(object sender, TextChangedEventArgs e)
        {
            PlayersFound.Clear();
            if (tbUserList.Text != "")
            {
                foreach (PlayerItem info in Players)
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
            _admin_SpecialChatMessage(style.GetGameColor("InformationMessageColor"), String.Format("Le chat a été nettoyé par {0}. Raison : {1}.", username, reason), true, true);
        }

        private void _admin_RemoveHubPlayer(PlayerInfo infos)
        {
            int index = -1;
            foreach (PlayerItem p in Players)
                if (p.UserId == infos.UserId)
                {
                    index = Players.IndexOf(p);
                    break;
                }

            if (index == -1)
                return;

            Players.RemoveAt(index);
            lvUserlist.Items.Refresh();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUserlist.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Rank");
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(groupDescription);

            if (FormExecution.ClientConfig.Connexion_Message)
                _admin_SpecialChatMessage(style.GetGameColor("LauncherMessageColor"), String.Format("{0} s'est déconnecté.", infos.Username), false, false);
            logger.Trace("{0} removed from userlist.", infos);
        }
        private void _admin_AddHubPlayer(PlayerInfo infos)
        {
            AddPlayer(infos);

            if (FormExecution.ClientConfig.Connexion_Message)
                _admin_SpecialChatMessage(style.GetGameColor("LauncherMessageColor"), String.Format("{0} s'est connecté.", infos.Username), false, false);
            logger.Trace("{0} added to userlist.", infos);
        }

        private void _admin_LoginComplete()
        {
            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.RenderTransform = scale;

            DoubleAnimation scaley = new DoubleAnimation();
            scaley.Duration = TimeSpan.FromMilliseconds(200);
            scaley.From = 0.0;
            scaley.To = 1.0;
            storyboard.Children.Add(scaley);

            Storyboard.SetTargetProperty(scaley, new PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTarget(scaley, this);

            storyboard.Begin();

            Show();
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();            

            cb_defaultdeck.Items.Clear();
            List<string> Deck = new List<string>(Directory.EnumerateFiles(Path.Combine(FormExecution.path, "BattleCityAlpha", "deck")));
            foreach (string deck in Deck)
            {
                string[] name = deck.Split('\\');
                string[] nomFinal = name[name.Length - 1].Split('.');
                cb_defaultdeck.Items.Add(nomFinal[0]);
            }
            cb_defaultdeck.Text = YgoproConfig.GetDefaultDeck();

            tb_version.Text = FormExecution.Username + " - " + Main.VERSION + "c" + FormExecution.ClientConfig.CardsStuffVersion;
            
            logger.Trace("Style loaded.");
        }

        public void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btnArene, btnShop, btnDecks, btnAnimations, btnTools, btnProfil, btnFAQ, btnReplay, btnNote, btnDiscord, btnRules, btnForum });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1HomeHeadButton");
                btn.Color2 = style.GetGameColor("Color2HomeHeadButton");
                btn.Update();
            }

            this.FontFamily = style.Font;
            this.chat.chat.FontSize = style.FontSize;

            Resources.Clear();

            Style s = new Style(typeof(Control));
            s.Setters.Add(new Setter(Control.FontFamilyProperty, style.Font));
            Resources.Add(typeof(Control), style);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnDiscord.ClickedAnimation();
            System.Diagnostics.Process.Start("https://discordapp.com/invite/EYkXU7N");
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
            note.Owner = this;
            note.Show();
            logger.Trace("Notes clicked.");
        }

        private void Image_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            btnNote.ReleasedAnimation();
        }

        private void btnFAQ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://forum.battlecityalpha.xyz/thread-681.html");
            logger.Trace("FAQ clicked.");
        }

        private void btnCGU_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            YgoProHelper.LoadCustomization(new Customization(CustomizationType.Avatar, 14, false, ""), new Customization(CustomizationType.Border, 1, false, ""), new Customization(CustomizationType.Sleeve, 203, false, ""), 0);
            YgoProHelper.LoadCustomization(new Customization(CustomizationType.Avatar, 14, false, ""), new Customization(CustomizationType.Border, 1, false, ""), new Customization(CustomizationType.Sleeve, 203, false, ""), 1);
            YgoProHelper.LaunchYgoPro("-r");
        }

        private void tbChat_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    _admin.SendMessage(tbChat.GetText());
                    _last_messages.Add(tbChat.GetText());
                    _index_last_message = _last_messages.Count;
                    tbChat.Clear();
                    break;
                case Key.Up:
                    if (_index_last_message != 0)
                        _index_last_message -= 1;
                    tbChat.SetText(_last_messages[_index_last_message]);
                    break;
                case Key.Down:
                    if (_index_last_message != _last_messages.Count - 1)
                        _index_last_message += 1;
                    tbChat.SetText(_last_messages[_index_last_message]);
                    break;
            }
            e.Handled = true;
        }

        private string FindUsername(string txt)
        {
            foreach (PlayerItem i in Players)
                if (i.Username.ToLower().StartsWith(txt.ToLower()))
                    return i.Username;
            return txt;
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
            _admin.SendProfileAsking();
        }

        private void btnDecks_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            YgoProHelper.LoadCustomization(new Customization(CustomizationType.Avatar, 14, false, ""), new Customization(CustomizationType.Border, 1, false, ""), new Customization(CustomizationType.Sleeve, 203, false, ""), 0);
            YgoProHelper.LoadCustomization(new Customization(CustomizationType.Avatar, 14, false, ""), new Customization(CustomizationType.Border, 1, false, ""), new Customization(CustomizationType.Sleeve, 203, false, ""), 1);

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
            PlayerInfo target = lvUserlist.SelectedItem as PlayerInfo;
            if (target != null)
                FormExecution.OpenNewPrivateForm(target);
        }

        private void duelrequest_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = lvUserlist.SelectedItem as PlayerInfo;
            if (target != null)
                FormExecution.OpenDuelRequest(target.UserId);
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
                _admin_SpecialChatMessage(FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor"), String.Format("••• Vous avez ajouté à votre blacklist : {0}.", target.Username), false, false);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.SpecialChatMessage -= _admin_SpecialChatMessage;
            _admin.PlayerChatMessage -= _admin_PlayerChatMessage;
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
            PlayerItem item = CreatePlayerItem(infos);
            for (int i = 0; i < Players.Count; i++)
                if (Players[i].Rank <= item.Rank)
                {
                    Players.Insert(i, item);
                    break;
                }

            if (!Players.Contains(item))
                Players.Add(item);

            lvUserlist.Items.Refresh();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUserlist.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Rank");
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(groupDescription);
        }

        private void BtnRules_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forum.battlecityalpha.xyz/thread-20.html");
            logger.Trace("Rules clicked.");
        }

        private void BtnForum_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forum.battlecityalpha.xyz/");
            logger.Trace("Forum clicked.");
        }

        private void SendBP_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = ((PlayerInfo)lvUserlist.SelectedItem);

            if (target != null)
            {
                InputText form = new InputText();
                form.Title = "Don de BP à " + target.Username;
                form.SelectedText += (obj) => BpInputForm_SelectedText(obj, target);
                form.Owner = this;
                form.ShowDialog();
            }
        }
        private void BpInputForm_SelectedText(string obj, PlayerInfo target)
        {
            _admin.SendDonationBP(obj, target);
        }

        private void Regarder_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = ((PlayerInfo)lvUserlist.SelectedItem);
            if (target != null)
                _admin.SendSpectateRequest(target);
        }

        private void GiveCard_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = ((PlayerInfo)lvUserlist.SelectedItem);

            if (target != null)
            {
                _admin.AskSelectCard();
                SelectCard form = new SelectCard(_admin.Client.SelectCardAdmin);
                form.ActivateDonationCardMode();
                form.SelectedCard += (arg1, arg2, arg3) => SendGiveCard(arg1, arg2, arg3, target);
                form.Owner = this;
                form.ShowDialog();
                form.SelectedCard -= (arg1, arg2, arg3) => SendGiveCard(arg1, arg2, arg3, target);
            }
        }
        private void SendGiveCard(PlayerCard card, int price, int quantity, PlayerInfo target)
        {
            _admin.SendCardDonation(card, quantity, target);
        }

        private void cb_defaultdeck_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            YgoproConfig.UpdateDefaultDeck((string)cb_defaultdeck.SelectedItem);
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(40, 0, 40, 40);
            }
            else if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.bg_border.CornerRadius = new CornerRadius(0);
            }
        }
        private void minimizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Chat_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }

        private PlayerItem CreatePlayerItem(PlayerInfo infos)
        {
            PlayerItem item = new PlayerItem
            {
                ChatColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + infos.ChatColorString)),
                ELO = infos.ELO,
                Level = infos.Level,
                Rank = infos.Rank,
                ChatColorString = infos.ChatColorString,
                State = infos.State,
                UserId = infos.UserId,
                Username = infos.Username,
                VIP = infos.VIP
            };

            if (!infos.Avatar.IsHost)
                item.AvatarPic = PicsManager.GetImage("Avatars", infos.Avatar.Id.ToString());
            else
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += (sender, e) => userAvatarDownloaded(sender, e, item);
                        wc.DownloadFileAsync(
                            new System.Uri(infos.Avatar.URL),
                            Path.Combine(FormExecution.path, "Assets", "Avatars", "A_" + item.UserId + ".png")
                            );
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur", true);
                }
            }

            return item;
        }

        private void userAvatarDownloaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e, PlayerItem item)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(Path.Combine(FormExecution.path, "Assets", "Avatars", "A_" + item.UserId + ".png"));
            image.EndInit();

            item.AvatarPic = image;

            lvUserlist.Items.Refresh();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUserlist.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Rank");
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(groupDescription);

            //File.Delete(Path.Combine(FormExecution.path, "Assets", "Avatars", "A_" + item.UserId + ".png"));
        }

        private void profile_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = ((PlayerInfo)lvUserlist.SelectedItem);

            if (target != null)
            {
                Profil profile = new Profil(_admin.Client.ProfilAdmin);
                _admin.SendAskProfil(target);
            }
        }

        private string ParseGroup(PlayerRank group)
        {
            switch (group)
            {
                case PlayerRank.Administrateurs:
                    return "♛Administrateurs";
                case PlayerRank.Robots:
                    return "☎";
                case PlayerRank.Moderateurs:
                    return "♝Modérateurs";
                case PlayerRank.Animateurs:
                    return "♞Animateurs";
                case PlayerRank.Developpeurs:
                    return "♣Développeur";
                case PlayerRank.Rullers:
                    return "♟Rullers";
                default:
                    return group.ToString();
            }
        }

        private void shareDeck_Click(object sender, RoutedEventArgs e)
        {
            if (lvUserlist.SelectedIndex == -1) return;
            PlayerInfo target = ((PlayerInfo)lvUserlist.SelectedItem);

            if (target != null)
            {
                OpenFileDialog getdeck = new OpenFileDialog();
                getdeck.InitialDirectory = Path.Combine(FormExecution.path, "BattleCityAlpha", "decks");
                getdeck.Filter = "Deck files (*.ydk;)|*.ydk|All files (*.*)|*.*";
                if (getdeck.ShowDialog() == true)
                {
                    _admin.SendShareDeck(target, File.ReadAllLines(getdeck.FileName), Path.GetFileNameWithoutExtension(getdeck.FileName));
                }
            }
        }

        private void userlistAvatarPics_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;

            var item = VisualTreeHelper.HitTest(lvUserlist, Mouse.GetPosition(lvUserlist)).VisualHit;

            // find ListViewItem (or null)
            while (item != null && !(item is ListViewItem))
                item = VisualTreeHelper.GetParent(item);

            if (item == null)
                return;
            int i = lvUserlist.Items.IndexOf(((ListViewItem)item).DataContext);

            PlayerItem pitem = lvUserlist.Items[i] as PlayerItem;

            popup_username.Text = _admin.Client.ParseUsernames(pitem.Username, pitem.Rank, pitem.VIP);
            popup_username.Foreground = pitem.ChatColor;
            popup_avatar.Background = border.Background;

            popup_border.Opacity = 0;
            popup_border.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 100, new Duration(new TimeSpan(0, 0, 0, 1, 0))));
            profil_popup.IsOpen = true;
        }

        private void userlistAvatarPics_MouseLeave(object sender, MouseEventArgs e)
        {
            popup_border.BeginAnimation(OpacityProperty, new DoubleAnimation(100, 0, new Duration(new TimeSpan(0, 0, 0, 1, 0))));
            profil_popup.IsOpen = false;
        }
    }
}