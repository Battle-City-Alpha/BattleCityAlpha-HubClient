using BCA.Common;
using hub_client.Assets;
using hub_client.Stuff;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.IO;
using System.Net;
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

        AssetsManager PicsManager = new AssetsManager();
        public RankingWindow(RankingDisplayAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _admin = admin;

            _admin.ShowRanking += _admin_ShowRanking;

            this.MouseDown += Window_MouseDown;
            this.Closed += RankingWindow_Closed;

            this.lvRanking.MouseDoubleClick += LvRanking_MouseDoubleClick;

            this.img_left.MouseLeftButtonDown += PreviousSeason;
            this.img_right.MouseLeftButtonDown += NextSeason;
        }

        private void NextSeason(object sender, MouseButtonEventArgs e)
        {
            _seasonOffset++;
            _admin.SendGetRanking(_seasonOffset);
        }

        private void PreviousSeason(object sender, MouseButtonEventArgs e)
        {
            _seasonOffset--;
            _admin.SendGetRanking(_seasonOffset);
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
        }

        private void _admin_ShowRanking(RankingPlayerInfos[] infos, Customization[] customs, int season)
        {
            lblSeason.Content = season;
            lvRanking.Items.Clear();
            Border[] borders = new Border[3] { bg_first, bg_second, bg_third };
            TextBlock[] textblocks = new TextBlock[3] { tb_first, tb_second, tb_third };

            for (int i = 0; i < 3; i++)
            {
                textblocks[i].Text = "NR";
                borders[i].Background = null;
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

            for (int i = 0; i < customs.Length; i++)
            {
                textblocks[i].Text = infos[i].Username;
                borders[i].Background = new ImageBrush(PicsManager.GetCustom(customs[i]));                
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
