﻿using BCA.Common;
using BCA.Common.Bets;
using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Helpers;
using hub_client.Windows.Controls;
using hub_client.Windows.Controls.Controls_Stuff;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Arena.xaml
    /// </summary>
    public partial class Arena : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        ArenaAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        public DispatcherTimer RankedTimer;
        private int _rankedTimerCounter;
        private bool _isOverRankedBtn = false;

        private bool _availableRooms = true;

        private int _tutoIndex = 0;

        public Arena(ArenaAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _admin = admin;

            foreach (var room in _admin.WaitingRooms)
                UpdateRoom(room.Value);

            Title = "Arène de duel";
            this.FontFamily = style.Font;

            _admin.UpdateRoom += UpdateRoom;

            singleList.Itemslist.MouseDoubleClick += Room_MouseDoubleClick;
            matchList.Itemslist.MouseDoubleClick += Room_MouseDoubleClick;
            tagList.Itemslist.MouseDoubleClick += Room_MouseDoubleClick;

            singleList.Itemslist.MouseMove += Room_MouseEnter;
            matchList.Itemslist.MouseMove += Room_MouseEnter;
            tagList.Itemslist.MouseMove += Room_MouseEnter;
            singleList.Itemslist.MouseLeave += Room_MouseLeave;
            matchList.Itemslist.MouseLeave += Room_MouseLeave;
            tagList.Itemslist.MouseLeave += Room_MouseLeave;

            this.Loaded += Arena_Loaded;
            this.MouseDown += Window_MouseDown;

            RankedTimer = new DispatcherTimer();
            RankedTimer.Interval = TimeSpan.FromSeconds(1);
            RankedTimer.Tick += RankedTimer_Tick;
            RankedTimer.IsEnabled = false;

            duel_popup.MouseEnter += Duel_popup_MouseEnter;
        }

        private void Duel_popup_MouseEnter(object sender, MouseEventArgs e)
        {
            if (duel_popup.IsOpen)
                duel_popup.IsOpen = false;
        }

        private void Room_MouseLeave(object sender, MouseEventArgs e)
        {
            if (duel_popup.IsOpen && !duel_popup.IsMouseOver)
                duel_popup.IsOpen = false;
        }

        private void Room_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                ListBox Itemslist = ((ListBox)sender);
                var item = VisualTreeHelper.HitTest(Itemslist, Mouse.GetPosition(Itemslist)).VisualHit;

                // find ListViewItem (or null)
                while (item != null && !(item is ListBoxItem))
                    item = VisualTreeHelper.GetParent(item);

                if (item == null)
                    return;

                int i = Itemslist.Items.IndexOf(((ListBoxItem)item).DataContext);
                RoomItem room = Itemslist.Items[i] as RoomItem;
                if (room != null)
                {
                    //popup_dueltype_img.Background = new ImageBrush(room.Image);

                    duel_popup.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                    duel_popup.HorizontalOffset = 10;
                    duel_popup.VerticalOffset = 10;

                    tb_popup_banlist.Foreground = new SolidColorBrush(Colors.Black);
                    tb_popup_lp.Foreground = new SolidColorBrush(Colors.Black);
                    tb_popup_MR.Foreground = new SolidColorBrush(Colors.Black);
                    tb_popup_starthand.Foreground = new SolidColorBrush(Colors.Black);
                    tb_shuffledeck.Foreground = new SolidColorBrush(Colors.Black);
                    tb_drawcount.Foreground = new SolidColorBrush(Colors.Black);
                    tb_info.Foreground = new SolidColorBrush(Colors.Black);
                    tb_captiontext.Foreground = new SolidColorBrush(Colors.Black);

                    if (room.Config.Banlist != 0)
                        tb_popup_banlist.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
                    tb_popup_banlist.Text = FormExecution.GetBanlistValue(room.Config.Banlist);

                    if ((room.Config.StartDuelLP != 8000 && room.Type != RoomType.Tag) || (room.Config.StartDuelLP != 16000 && room.Type == RoomType.Tag))
                        tb_popup_lp.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
                    tb_popup_lp.Text = room.Config.StartDuelLP.ToString();

                    if (room.Config.MasterRules != 5)
                        tb_popup_MR.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
                    if (room.IsTurboDuel)
                    {
                        tb_popup_MR_title.Text = "🎮 Turbo Duel 🎮";
                        tb_popup_MR.Text = "";
                        tb_popup_MR_title.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("TurboDuelRoomColor"));
                        Grid.SetColumnSpan(tb_popup_MR_title, 2);
                    }
                    else
                    {
                        tb_popup_MR_title.Text = "Master Rules : ";
                        tb_popup_MR.Text = room.Config.MasterRules.ToString();
                    }

                    tb_popup_players1.Text = room.Players1;
                    tb_popup_players2.Text = room.Players2;

                    if (room.Config.CardByHand != 5)
                        tb_popup_starthand.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
                    tb_popup_starthand.Text = room.Config.CardByHand.ToString();

                    if (room.Config.DrawCount != 1)
                        tb_drawcount.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
                    tb_drawcount.Text = room.Config.DrawCount.ToString();

                    if (room.Config.NoShuffleDeck)
                    {
                        tb_shuffledeck.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
                        tb_shuffledeck.Text = "Deck non mélangé";
                    }

                    if (room.IsShadowDuel)
                    {
                        this.tb_info.Text = "Mise : ";

                        switch (room.Bet.BType)
                        {
                            case BetType.BPs:
                                this.tb_captiontext.Text = ((BPsBet)room.Bet).Amount + " BPs";
                                break;
                            case BetType.Mute:
                                this.tb_captiontext.Text = "Mute " + ((SanctionBet)room.Bet).Time + "h";
                                break;
                            case BetType.Ban:
                                this.tb_captiontext.Text = "Ban " + ((SanctionBet)room.Bet).Time + "h";
                                break;
                        }

                        this.tb_info.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("ShadowRoomColor"));
                        this.tb_captiontext.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("ShadowRoomColor"));
                    }
                    else
                        tb_captiontext.Text = room.Config.CaptionText;

                    tb_popup_type.Text = room.Type.ToString();


                    duel_popup.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
            }
        }

        private void Arena_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
            if (FormExecution.ClientConfig.DoTutoArena)
            {
                BCA_TutoPopup tutopopup = new BCA_TutoPopup();
                maingrid.Children.Add(tutopopup);
                tutopopup.HorizontalAlignment = HorizontalAlignment.Center;
                tutopopup.VerticalAlignment = VerticalAlignment.Center;
                tutopopup.tuto_popup.IsOpen = true;
                tutopopup.tuto_popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
                tutopopup.tuto_popup.PlacementTarget = maingrid;
                tutopopup.SetText(StartDisclaimer.ArenaTutorial[_tutoIndex]);
                tutopopup.tuto_popup.MaxWidth = this.Width - 200;

                tutopopup.SkipTuto += SkipTutorial;
                tutopopup.NextStep += Tutopopup_NextStep;
            }
        }

        private void Tutopopup_NextStep(BCA_TutoPopup popup)
        {
            _tutoIndex++;

            if (_tutoIndex == StartDisclaimer.ArenaTutorial.Length - 1)
            {
                popup.btnNext.Visibility = Visibility.Hidden;
                popup.btnSkip.ButtonText = "Fin !";
                popup.btnSkip.Update();
            }

            switch (_tutoIndex)
            {
                case 1:
                    SetTutorialColor(btn_ranking);
                    SetTutorialColor(btn_playranked);
                    break;
                case 2:
                    SetOriginalColor(btn_ranking);
                    SetOriginalColor(btn_playranked);
                    SetTutorialColor(btn_host);
                    break;
                case 3:
                    SetOriginalColor(btn_host);
                    SetTutorialColor(btn_IA);
                    break;
                case 4:
                    SetOriginalColor(btn_IA);
                    SetTutorialColor(btn_Switch_Rooms_Visible);
                    break;
                case 5:
                    SetOriginalColor(btn_Switch_Rooms_Visible);
                    break;
            }
            if (_tutoIndex >= StartDisclaimer.ArenaTutorial.Length)
                SkipTutorial(popup);
            else
                popup.SetText(StartDisclaimer.ArenaTutorial[_tutoIndex]);
        }

        private void SkipTutorial(BCA_TutoPopup popup)
        {
            popup.tuto_popup.IsOpen = false;
            LoadStyle();
            FormExecution.ClientConfig.DoTutoArena = false;
            FormExecution.ClientConfig.Save();
        }
        private void SetTutorialColor(BCA_ColorButton btn)
        {
            btn.Color1 = Colors.Blue;
            btn.Color2 = Colors.BlueViolet;
            btn.Update();
        }
        private void SetOriginalColor(BCA_ColorButton btn)
        {
            btn.Color1 = style.GetGameColor("Color1ArenaButton");
            btn.Color2 = style.GetGameColor("Color2ArenaButton");
            btn.Update();
        }

        private void Room_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RoomItem room = ((ListBox)sender).SelectedItem as RoomItem;
            if (room != null)
            {
                if (!room.NeedPassword)
                    _admin.SendJoinRoom(room.Id, room.Type, "");
                else
                {
                    InputText form = new InputText("mot de passe...");
                    form.Title = "Mot de passe";
                    form.SelectedText += (obj) => RoomPassInput_SelectedText(obj, room);
                    form.Topmost = true;
                    form.Show();
                    Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => form.Activate()));
                }
            }
        }

        private void RoomPassInput_SelectedText(string pass, RoomItem room)
        {
            _admin.SendJoinRoom(room.Id, room.Type, pass);
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> RankedButtons = new List<BCA_ColorButton>();
            RankedButtons.AddRange(new[] { btn_playranked, btn_ranking, btn_host, btn_IA, btn_Switch_Rooms_Visible });

            foreach (BCA_ColorButton btn in RankedButtons)
            {
                btn.Color1 = style.GetGameColor("Color1ArenaButton");
                btn.Color2 = style.GetGameColor("Color2ArenaButton");
                btn.Update();
            }
        }

        private void UpdateRooms()
        {
            singleList.Clear();
            matchList.Clear();
            tagList.Clear();

            Dictionary<int, Room> rooms = _admin.WaitingRooms;
            if (!_availableRooms)
                rooms = _admin.DuelingRooms;

            foreach (var val in rooms)
                UpdateRoom(val.Value);
        }
        public void UpdateRoom(Room room)
        {
            switch (room.State)
            {
                case RoomState.Waiting:
                    if (!_availableRooms)
                        return;
                    UpdateRoomInList(room);
                    break;
                case RoomState.Dueling:
                    if (_availableRooms)
                    {
                        DeleteRoomInList(room);
                        return;
                    }
                    UpdateRoomInList(room);
                    break;
                case RoomState.Finished:
                    DeleteRoomInList(room);
                    break;
            }
        }
        private void UpdateRoomInList(Room room)
        {
            switch (room.Config.Type)
            {
                case RoomType.Single:
                    singleList.UpdateRoom(room);
                    break;
                case RoomType.Match:
                    matchList.UpdateRoom(room);
                    break;
                case RoomType.Tag:
                    tagList.UpdateRoom(room);
                    break;
            }
        }
        private void DeleteRoomInList(Room room)
        {
            switch (room.Config.Type)
            {
                case RoomType.Single:
                    singleList.RemoveRoom(room);
                    break;
                case RoomType.Match:
                    matchList.RemoveRoom(room);
                    break;
                case RoomType.Tag:
                    tagList.RemoveRoom(room);
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.UpdateRoom -= UpdateRoom;
        }

        private void Btn_host_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.OpenDuelRequest(-1);
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (RankedTimer.IsEnabled)
            {
                _admin.SendStopPlayRanked();
                StopTimer();
            }
            FormExecution.ActivateChat();
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
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }

        private void btn_IA_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            YgoProHelper.LaunchGameAgainstBot();
        }

        private void btn_playranked_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (FormExecution.ClientConfig.FirstTimeRanked)
            {
                FormExecution.Client_PopMessageBoxShowDialog(StartDisclaimer.RankedText, "Premièr duel classé !");
                FormExecution.ClientConfig.FirstTimeRanked = false;
                FormExecution.ClientConfig.Save();
            }

            if (RankedTimer.IsEnabled)
            {
                _admin.SendStopPlayRanked();
                StopTimer();
            }
            else
            {
                ChoicePopBox cpb = new ChoicePopBox(FormExecution.PlayerInfos, new RoomConfig(), ChoiceBoxType.DeckChoiceRanked, "", YgoproConfig.GetDefaultDeck());
                cpb.Choice += Cpb_Choice;
                cpb.Show();
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => cpb.Activate()));
            }
        }

        private void Cpb_Choice(bool result)
        {
            if (!result)
                return;

            _admin.SendPlayRanked();

            _rankedTimerCounter = 0;
            RankedTimer.IsEnabled = true;
            btn_playranked.MouseEnter += Btn_playranked_MouseEnter;
            btn_playranked.MouseLeave += Btn_playranked_MouseLeave;
        }

        private void RankedTimer_Tick(object sender, EventArgs e)
        {
            _rankedTimerCounter++;
            if (!_isOverRankedBtn)
                btn_playranked.text.Content = _rankedTimerCounter.ToString();
        }

        private void Btn_playranked_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_playranked.text.Content = _rankedTimerCounter.ToString();
            _isOverRankedBtn = false;
        }
        private void Btn_playranked_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_playranked.text.Content = "Stop";
            _isOverRankedBtn = true;
        }

        public void StopTimer()
        {
            RankedTimer.IsEnabled = false;
            btn_playranked.text.Content = "Jouer (classé)";
            btn_playranked.MouseEnter -= Btn_playranked_MouseEnter;
            btn_playranked.MouseLeave -= Btn_playranked_MouseLeave;
        }

        private void btn_Switch_Rooms_Visible_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_availableRooms)
                btn_Switch_Rooms_Visible.text.Content = "En cours";
            else
                btn_Switch_Rooms_Visible.text.Content = "Disponible";
            _availableRooms = !_availableRooms;

            UpdateRooms();
        }

        private void btn_ranking_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.SendGetRanking();
        }
    }
}
