using BCA.Common;
using hub_client.Windows.Controls.Controls_Stuff;
using hub_client.WindowsAdministrator;
using System;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour TeamGamesHistory.xaml
    /// </summary>
    public partial class TeamGamesHistory : Window
    {
        TeamGamesHistoryAdministrator _admin;

        public TeamGamesHistory(TeamGamesHistoryAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MouseDown += Window_MouseDown;
            this.Closed += TeamGamesHistory_Closed;

            _admin = admin;
            _admin.ShowResults += _admin_ShowResults;

        }

        private void TeamGamesHistory_Closed(object sender, EventArgs e)
        {
            _admin.ShowResults -= _admin_ShowResults;
        }

        private void _admin_ShowResults(TeamGameResult[] results)
        {
            foreach (TeamGameResult result in results)
            {
                TeamGameResultItem item = new TeamGameResultItem
                {
                    Winner = result.Winner.Username + " (" + result.Winner.TeamTag + ")",
                    WinnerAvatar = FormExecution.AssetsManager.GetCustom(result.Winner.Avatar),
                    WinnerTeam = FormExecution.AssetsManager.GetTeamEmblem(result.Winner.Team, result.Winner.TeamEmblem),
                    Looser = result.Looser.Username + " (" + result.Looser.TeamTag + ")",
                    LooserAvatar = FormExecution.AssetsManager.GetCustom(result.Looser.Avatar),
                    LooserTeam = FormExecution.AssetsManager.GetTeamEmblem(result.Looser.Team, result.Looser.TeamEmblem)
                };
                gamesList.Items.Add(item);
            }

            this.Show();
            this.Width = gamesList.ActualWidth + 50;
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
