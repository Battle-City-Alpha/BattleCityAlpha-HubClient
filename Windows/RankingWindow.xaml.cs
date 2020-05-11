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

        AssetsManager PicsManager = new AssetsManager();
        public RankingWindow(RankingDisplayAdministrator admin)
        {
            InitializeComponent();

            _admin = admin;

            _admin.ShowRanking += _admin_ShowRanking;

            this.MouseDown += Window_MouseDown;
            this.Closed += RankingWindow_Closed;

            this.lvRanking.MouseDoubleClick += LvRanking_MouseDoubleClick;
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

        private void _admin_ShowRanking(RankingPlayerInfos[] infos, Customization[] customs)
        {
            lvRanking.Items.Clear();
            foreach (RankingPlayerInfos info in infos)
            {
                RankingPlayerItem item = new RankingPlayerItem
                {
                    ELO = info.ELO,
                    Rank = info.Rank,
                    RankedLose = info.RankedLose,
                    RankedWin = info.RankedWin,
                    UserID = info.UserID,
                    Username = info.Username
                };
                if (info.RankedLose == 0 & info.RankedWin == 0)
                    item.WinRate = 1.0;
                else
                    item.WinRate = Math.Round(((double)info.RankedWin) / (info.RankedLose + info.RankedWin), 2);
                lvRanking.Items.Add(item);
            }

            tb_first.Text = infos[0].Username;
            tb_second.Text = infos[1].Username;
            tb_third.Text = infos[2].Username;

            Border[] borders = new Border[3] { bg_first, bg_second, bg_third };

            for (int i = 0; i < 3; i++)
            {
                if (!customs[i].IsHost)
                    borders[i].Background = new ImageBrush(PicsManager.GetImage("Avatars", customs[i].Id.ToString()));
                else
                {
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFile(
                                new System.Uri(customs[i].URL),
                                Path.Combine(FormExecution.path, "Assets", "Avatars", "temp.png")
                                );
                        }
                        borders[i].Background = new ImageBrush(PicsManager.GetImage("Avatars", "temp"));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur", true);
                    }
                }
            }

            this.Show();
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
