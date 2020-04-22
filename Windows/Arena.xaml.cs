using BCA.Common;
using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.Windows.Controls.Controls_Stuff;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Arena.xaml
    /// </summary>
    public partial class Arena : Window
    {
        ArenaAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        public DispatcherTimer RankedTimer;
        private int _rankedTimerCounter;
        private bool _isOverRankedBtn = false;

        private bool _availableRooms = true;
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

            this.Loaded += Arena_Loaded;
            this.MouseDown += Window_MouseDown;

            RankedTimer = new DispatcherTimer();
            RankedTimer.Interval = TimeSpan.FromSeconds(1);
            RankedTimer.Tick += RankedTimer_Tick;
            RankedTimer.IsEnabled = false;
        }

        private void Arena_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
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
                    InputText form = new InputText();
                    form.Title = "Mot de passe";
                    form.SelectedText += (obj) => RoomPassInput_SelectedText(obj, room);
                    form.Owner = this;
                    form.ShowDialog();
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
            RankedButtons.AddRange(new[] { btn_playranked, btn_ranking, btn_host });

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
            FormExecution.OpenSoloModeWindow();
        }

        private void btn_playranked_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (RankedTimer.IsEnabled)
            {
                _admin.SendStopPlayRanked();
                StopTimer();
            }
            else
            {
                _admin.SendPlayRanked();

                _rankedTimerCounter = 0;
                RankedTimer.IsEnabled = true;
                btn_playranked.MouseEnter += Btn_playranked_MouseEnter;
                btn_playranked.MouseLeave += Btn_playranked_MouseLeave;
            }
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
    }
}
