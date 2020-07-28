using BCA.Common;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour TeamProfile.xaml
    /// </summary>
    public partial class TeamProfile : Window
    {
        private TeamProfileAdministrator _admin;
        private int _id;

        DoubleAnimation fadeInBorder;
        DoubleAnimation fadeOutBorder;
        DoubleAnimation fadeInText;
        DoubleAnimation fadeOutText;

        public TeamProfile(TeamProfileAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MouseDown += Window_MouseDown;

            _admin = admin;
            _admin.LoadTeamProfile += _admin_LoadTeamProfile;

            this.Closed += TeamProfile_Closed;

            fadeInBorder = new DoubleAnimation();
            fadeInBorder.From = 0.3;
            fadeInBorder.To = 1;
            fadeInBorder.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            fadeOutBorder = new DoubleAnimation();
            fadeOutBorder.From = 1;
            fadeOutBorder.To = 0.3;
            fadeOutBorder.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            fadeInText = new DoubleAnimation();
            fadeInText.From = 0;
            fadeInText.To = 1;
            fadeInText.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            fadeOutText = new DoubleAnimation();
            fadeOutText.From = 1;
            fadeOutText.To = 0;
            fadeOutText.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            this.team_emblem.MouseLeftButtonDown += AskTeamGamesHistory;
        }

        private void AskTeamGamesHistory(object sender, MouseButtonEventArgs e)
        {
            _admin.SendAskTeamGamesHistory(_id);
        }

        private void UpdateEmblem(object sender, MouseButtonEventArgs e)
        {
            string[] args = team_name.Text.Split('(');
            CreateTeam ct = new CreateTeam(args[0].Split(' ')[0], args[1].Split(')')[0]);
            ct.Show();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => ct.Activate()));

            ct.TeamCreation += UpdateEmblem;
        }

        private void UpdateEmblem(string name, string emblem, string tag)
        {
            _admin.SendUpdateTeamEmblem(emblem);
        }

        private void TeamProfile_Closed(object sender, EventArgs e)
        {
            _admin.LoadTeamProfile -= _admin_LoadTeamProfile;
        }

        private void _admin_LoadTeamProfile(int id, string url_emblem, string name, string tag, int wins, int loses, int rank, int leaderid, int coleaderid, PlayerInfo[] members, int score, Dictionary<int, int[]> stats)
        {
            _id = id;
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
                    winleader.Text = stats[p.UserId][0] + "W";
                    loseleader.Text = stats[p.UserId][1] + "L";
                    leader_avatar.Cursor = Cursors.Hand;
                    leader_avatar.MouseEnter += Leader_avatar_MouseEnter;
                    leader_avatar.MouseLeave += Leader_avatar_MouseLeave;
                    leader_avatar.MouseLeftButtonDown += (sender, e) => TeamMemberClick(sender, e, p.UserId);
                }
                else
                {
                    Grid gd = CreateMemberGrid(p, p.UserId == coleaderid, stats[p.UserId]);
                    int x = 0;
                    int y = 0;
                    switch (i)
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

            if (FormExecution.PlayerInfos.UserId == leaderid)
            {
                team_emblem.MouseRightButtonDown += UpdateEmblem;
            }

            this.Show();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => this.Activate()));
        }

        private void Leader_avatar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (winleader.IsMouseOver || loseleader.IsMouseOver)
                return;

            fadeInBorder.From = ((Border)sender).Opacity;
            fadeOutText.From = stats_leader.Opacity;

            stats_leader.BeginAnimation(OpacityProperty, fadeOutText);
            ((Border)sender).BeginAnimation(OpacityProperty, fadeInBorder);
        }

        private void Leader_avatar_MouseEnter(object sender, MouseEventArgs e)
        {
            fadeOutBorder.From = ((Border)sender).Opacity;
            fadeInText.From = stats_leader.Opacity;

            stats_leader.BeginAnimation(OpacityProperty, fadeInText);
            ((Border)sender).BeginAnimation(OpacityProperty, fadeOutBorder);
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

        private Grid CreateMemberGrid(PlayerInfo player, bool isColeader, int[] stats)
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

            StackPanel panelStats = new StackPanel();
            panelStats.Orientation = Orientation.Vertical;
            panelStats.HorizontalAlignment = HorizontalAlignment.Center;
            panelStats.VerticalAlignment = VerticalAlignment.Center;
            TextBlock win = new TextBlock();
            win.Text = stats[0] + "W";
            win.FontSize = 20;
            win.HorizontalAlignment = HorizontalAlignment.Center;
            win.VerticalAlignment = VerticalAlignment.Center;
            TextBlock lose = new TextBlock();
            lose.Text = stats[1] + "L";
            lose.FontSize = 20;
            lose.HorizontalAlignment = HorizontalAlignment.Center;
            lose.VerticalAlignment = VerticalAlignment.Center;
            panelStats.Children.Add(win);
            panelStats.Children.Add(lose);
            panelStats.Opacity = 0.0;
            Grid.SetRow(panelStats, 0);

            g.Children.Add(bd);
            g.Children.Add(tb);
            g.Children.Add(panelStats);

            bd.Cursor = Cursors.Hand;
            bd.MouseEnter += (sender, e) => Bd_MouseEnter(sender, e, panelStats);
            bd.MouseLeave += (sender, e) => Bd_MouseLeave(sender, e, panelStats);
            bd.MouseLeftButtonDown += (sender, e) => TeamMemberClick(sender, e, player.UserId);

            return g;
        }

        private void TeamMemberClick(object sender, MouseButtonEventArgs e, int userID)
        {
            _admin.SendAskTeamMemberGamesHistory(userID);
        }

        private void Bd_MouseLeave(object sender, MouseEventArgs e, StackPanel stats)
        {
            if (stats.Children[0].IsMouseOver || stats.Children[1].IsMouseOver)
                return;

            fadeInBorder.From = ((Border)sender).Opacity;
            fadeOutText.From = stats.Opacity;

            stats.BeginAnimation(OpacityProperty, fadeOutText);
            ((Border)sender).BeginAnimation(OpacityProperty, fadeInBorder);
        }

        private void Bd_MouseEnter(object sender, MouseEventArgs e, StackPanel stats)
        {
            fadeOutBorder.From = ((Border)sender).Opacity;
            fadeInText.From = stats.Opacity;

            stats.BeginAnimation(OpacityProperty, fadeInText);
            ((Border)sender).BeginAnimation(OpacityProperty, fadeOutBorder);
        }
    }
}
