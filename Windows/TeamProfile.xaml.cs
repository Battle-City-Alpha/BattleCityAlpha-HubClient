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
            _admin.LoadTeamProfile += _admin_LoadTeamProfile;

            this.Closed += TeamProfile_Closed;
        }

        private void TeamProfile_Closed(object sender, EventArgs e)
        {
            _admin.LoadTeamProfile -= _admin_LoadTeamProfile;
        }

        private void _admin_LoadTeamProfile(int id, string url_emblem, string name, string tag, int wins, int loses, int rank, int leaderid, int coleaderid, PlayerInfo[] members, int score)
        {
            team_name.Text = name + " (" + tag + ")";
            tb_score.Text = score.ToString();
            tb_wins.Text = wins.ToString();
            tb_loses.Text = loses.ToString();
            if (rank > 0)
                tb_rank.Text = rank.ToString();

            ImageBrush emblem = new ImageBrush(FormExecution.AssetsManager.GetTeamEmblem(id, url_emblem));
            emblem.Stretch = Stretch.UniformToFill;
            team_emblem.Background = emblem;

            int i = 0;
            foreach (PlayerInfo p in members)
            {
                if (p.UserId == leaderid)
                {
                    leader_name.Text = p.Username;
                    ImageBrush bg = new ImageBrush(FormExecution.AssetsManager.GetCustom(p.Avatar));
                    bg.Stretch = Stretch.UniformToFill;
                    leader_avatar.Background = bg;
                }
                else
                {
                    Grid gd = CreateMemberGrid(p, p.UserId == coleaderid);
                    int x = 0;
                    int y = 0;
                    switch(i)
                    {
                        case 0:
                            break;
                        case 1:
                            x = 1;
                            break;
                        case 2:
                            x = 2;
                            break;
                        case 3:
                            y = 1;
                            break;
                        case 4:
                            x = 1;
                            y = 1;
                            break;
                        case 5:
                            x = 2;
                            y = 1;
                            break;
                        case 6:
                            x = 3;
                            y = 1;
                            break;
                        case 7:
                            x = 4;
                            y = 1;
                            break;
                    }
                    if (y == 0)
                        grid_first_line.Children.Add(gd);
                    else
                        grid_second_line.Children.Add(gd);
                    Grid.SetColumn(gd, x);
                    i++;
                }
            }

            this.Show();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => this.Activate()));
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
            bd.Background = background;
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
