using BCA.Common;
using hub_client.Assets;
using hub_client.Stuff;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour RankingWindow.xaml
    /// </summary>
    public partial class RankingWindow : Window
    {
        private RankingDisplayAdministrator _admin;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private int _seasonOffset = 0;

        AssetsManager PicsManager = FormExecution.AssetsManager;
        public RankingWindow(RankingDisplayAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _admin = admin;

            _admin.ShowRanking += _admin_ShowRanking;
            _admin.ShowTeamsRanking += _admin_ShowTeamsRanking;

            this.MouseDown += Window_MouseDown;
            this.Closed += RankingWindow_Closed;

            this.lvRanking.MouseDoubleClick += LvRanking_MouseDoubleClick;
            this.lvTeamsRanking.MouseDoubleClick += LvTeamsRanking_MouseDoubleClick;

            this.img_left.MouseLeftButtonDown += PreviousSeason;
            this.img_right.MouseLeftButtonDown += NextSeason;

            rb_teams.Checked += RB_CheckedChange;
            rb_players.Checked += RB_CheckedChange;
        }

        private void LvTeamsRanking_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvTeamsRanking.SelectedIndex == -1) return;
            RankingTeamItem target = lvTeamsRanking.SelectedItem as RankingTeamItem;
            if (target != null)
            {
                _admin.SendAskTeamProfile(target.TeamID);
            }
        }

        private void _admin_ShowTeamsRanking(RankingTeamInfos[] rankings, int season)
        {
            lblSeason.Content = season;
            lvRanking.Items.Clear();
            lvTeamsRanking.Items.Clear();
            Border[] borders = new Border[3] { bg_first, bg_second, bg_third };
            Border[] frames = new Border[3] { frame_first, frame_second, frame_third };
            TextBlock[] textblocks = new TextBlock[3] { tb_first, tb_second, tb_third };
            StackPanel[] panels = new StackPanel[3] { panel_first, panel_second, panel_third };

            for (int i = 0; i < 3; i++)
            {
                panels[i].Visibility = Visibility.Hidden;
                textblocks[i].Text = "NR";
                borders[i].Background = null;
                frames[i].Background = new SolidColorBrush(Colors.White);
            }
            foreach (RankingTeamInfos info in rankings)
            {
                RankingTeamItem item = new RankingTeamItem
                {
                    Name = info.Name,
                    Wins = info.Wins,
                    Loses = info.Loses,
                    Rank = info.Rank,
                    Score = info.Score,
                    TeamID = info.TeamID
                };

                lvTeamsRanking.Items.Add(item);
            }

            for (int i = 0; i < Math.Min(3, rankings.Length); i++)
            {
                ImageBrush bg = new ImageBrush(PicsManager.GetTeamEmblem(rankings[i].TeamID, rankings[i].Emblem));
                bg.Stretch = Stretch.UniformToFill;
                frames[i].Background = bg;
                frames[i].CornerRadius = new CornerRadius(10, 10, 50, 50);
            }

            bd_playersranking.Visibility = Visibility.Hidden;
            bd_teamsrankings.Visibility = Visibility.Visible;

            this.Show();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => this.Activate()));
        }

        private void RB_CheckedChange(object sender, RoutedEventArgs e)
        {
            if ((bool)rb_players.IsChecked)
                _admin.SendGetRanking(_seasonOffset);
            else
                _admin.SendGetTeamRanking(_seasonOffset);
        }

        private void NextSeason(object sender, MouseButtonEventArgs e)
        {
            _seasonOffset++;
            if ((bool)rb_players.IsChecked)
                _admin.SendGetRanking(_seasonOffset);
            else
                _admin.SendGetTeamRanking(_seasonOffset);
        }

        private void PreviousSeason(object sender, MouseButtonEventArgs e)
        {
            _seasonOffset--;
            if ((bool)rb_players.IsChecked)
                _admin.SendGetRanking(_seasonOffset);
            else
                _admin.SendGetTeamRanking(_seasonOffset);
        }

        private void LvRanking_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvRanking.SelectedIndex == -1) return;
            RankingPlayerItem target = lvRanking.SelectedItem as RankingPlayerItem;
            if (target != null)
            {
                Profil profile = new Profil(_admin.Client.ProfilAdmin);
                _admin.SendAskProfil(target.UserID);
            }
        }

        private void RankingWindow_Closed(object sender, EventArgs e)
        {
            _admin.ShowRanking -= _admin_ShowRanking;
            _admin.ShowTeamsRanking -= _admin_ShowTeamsRanking;
        }

        private void _admin_ShowRanking(RankingPlayerInfos[] infos, Customization[] customs, int season)
        {
            lblSeason.Content = season;
            lvRanking.Items.Clear();
            lvTeamsRanking.Items.Clear();
            Border[] borders = new Border[3] { bg_first, bg_second, bg_third };
            Border[] frames = new Border[3] { frame_first, frame_second, frame_third };
            TextBlock[] textblocks = new TextBlock[3] { tb_first, tb_second, tb_third };
            StackPanel[] panels = new StackPanel[3] { panel_first, panel_second, panel_third };

            for (int i = 0; i < 3; i++)
            {
                panels[i].Visibility = Visibility.Visible;
                textblocks[i].Text = "NR";
                borders[i].Background = null;
                frames[i].Background = new SolidColorBrush(Colors.White);
            }
            foreach (RankingPlayerInfos info in infos)
            {
                RankingPlayerItem item = new RankingPlayerItem
                {
                    ELO = info.ELO,
                    Rank = info.Rank,
                    RankedLose = info.RankedLose,
                    RankedWin = info.RankedWin,
                    UserID = info.UserID,
                    Username = info.Username,
                    Team = info.Team
                };
                if (info.RankedLose == 0 & info.RankedWin == 0)
                    item.WinRate = 1.0;
                else
                    item.WinRate = Math.Round(((double)info.RankedWin) / (info.RankedLose + info.RankedWin), 2);
                lvRanking.Items.Add(item);
            }

            bd_playersranking.Visibility = Visibility.Visible;
            bd_teamsrankings.Visibility = Visibility.Hidden;

            for (int i = 0; i < customs.Length; i++)
            {
                textblocks[i].Text = infos[i].Username;
                borders[i].Background = new ImageBrush(PicsManager.GetCustom(customs[i]));
                frames[i].CornerRadius = new CornerRadius(50, 50, 10, 10);
                borders[i].CornerRadius = new CornerRadius(200);
            }

            this.Show();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => this.Activate()));
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
