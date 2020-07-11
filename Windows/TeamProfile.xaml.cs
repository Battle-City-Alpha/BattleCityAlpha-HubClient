using BCA.Common;
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
    /// Logique d'interaction pour TeamProfile.xaml
    /// </summary>
    public partial class TeamProfile : Window
    {
        private TeamProfileAdministrator _admin;

        public TeamProfile(TeamProfileAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MouseDown += Window_MouseDown;

            _admin = admin;
        }


        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
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

        private Grid CreateMemberGrid(PlayerInfo player, bool isColeader)
        {
            Grid g = new Grid();
            g.RowDefinitions.Add(new RowDefinition());
            RowDefinition r = new RowDefinition();
            r.Height = new GridLength(20);
            g.RowDefinitions.Add(r);
            Border bd = new Border();
            bd.CornerRadius = new CornerRadius(300);
            bd.VerticalAlignment = VerticalAlignment.Center;
            bd.BorderBrush = new SolidColorBrush(Colors.Black);
            bd.Width = 100;
            bd.Height = 100;
            ImageBrush background = new ImageBrush();
            background.ImageSource = FormExecution.AssetsManager.GetCustom(player.Avatar);
            background.Stretch = Stretch.UniformToFill;
            Grid.SetRow(bd, 0);
            TextBlock tb = new TextBlock();
            tb.Text = isColeader ? "🍩" + player.Username : player.Username;
            tb.FontSize = 15;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(tb, 1);
            g.Children.Add(bd);
            g.Children.Add(tb);
            return g;
        }
    }
}
