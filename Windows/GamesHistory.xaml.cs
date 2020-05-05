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
    /// Logique d'interaction pour GamesHistory.xaml
    /// </summary>
    public partial class GamesHistory : Window
    {
        private GamesHistoryAdministrator _admin;
        public GamesHistory(GamesHistoryAdministrator admin)
        {
            InitializeComponent();

            _admin = admin;
            _admin.GetGamesHistory += _admin_GetGamesHistory;

            this.MouseDown += Window_MouseDown;
            this.Closed += GamesHistory_Closed;
        }

        private void GamesHistory_Closed(object sender, EventArgs e)
        {
            _admin.GetGamesHistory -= _admin_GetGamesHistory;
        }

        private void _admin_GetGamesHistory(RoomResultItem[] items)
        {
            foreach (RoomResultItem item in items)
                gamesList.Items.Add(item);

            this.Show();
            this.Activate();
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
    }
}
