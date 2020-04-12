using BCA.Common;
using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.Windows.Controls.Controls_Stuff;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Arena.xaml
    /// </summary>
    public partial class Arena : Window
    {
        ArenaAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        public Arena(ArenaAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;
            Title = "Arène de duel";

            LoadStyle();
            this.FontFamily = style.Font;

            _admin.UpdateRoom += UpdateRoom;

            singleList.Itemslist.MouseDoubleClick += Room_MouseDoubleClick;

            this.MouseDown += Window_MouseDown;
        }

        private void Room_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RoomItem room = ((ListBox)sender).SelectedItem as RoomItem;
            if (room != null)
                _admin.SendJoinRoom(room.Id, room.Type);
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

        public void UpdateRoom(Room room, bool remove)
        {
            if (!remove)
            {
                switch(room.Config.Type)
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
            else
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
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btn_IA_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.OpenSoloModeWindow();
        }
    }
}
