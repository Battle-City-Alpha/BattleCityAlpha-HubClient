using BCA.Network.Packets.Standard.FromServer;
using hub_client.Assets;
using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Profil.xaml
    /// </summary>
    public partial class Profil : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ProfilAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        private int _userID = -1;

        AssetsManager PicsManager = FormExecution.AssetsManager;

        private bool IsMine()
        {
            return FormExecution.PlayerInfos.Username.ToUpper() == this.tb_username.Text.ToUpper();
        }

        public Profil(ProfilAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _admin = admin;

            _admin.UpdateProfil += _admin_UpdateProfil;

            this.FontFamily = FormExecution.AppDesignConfig.Font;

            this.MouseDown += Window_MouseDown;

            img_partner.MouseLeftButtonDown += Img_partner_MouseLeftButtonDown;
        }

        private void Img_partner_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsMine())
                return;
            _admin.OpenPartnersForm(this);
        }

        private void _admin_UpdateProfil(StandardServerProfilInfo infos)
        {
            _userID = infos.UserID;

            img_avatar.Source = PicsManager.GetCustom(infos.Avatar);
            img_border.Source = PicsManager.GetCustom(infos.Border);
            img_sleeve.Source = PicsManager.GetCustom(infos.Sleeve);
            img_partner.Source = PicsManager.GetCustom(infos.Partner);

            tb_username.Text = infos.Username;
            tb_level.Text = infos.Level.ToString();
            tb_experience.Text = infos.Exp.ToString();
            tb_cardcount.Text = infos.CardNumber.ToString();

            tb_rankedwin.Text = infos.RankedWin.ToString();
            tb_rankedlose.Text = infos.RankedLose.ToString();
            tb_rankeddraw.Text = infos.RankedDraw.ToString();
            tb_elo.Text = infos.ELO.ToString();
            tb_rank.Text = infos.Rank.ToString();
            tb_ranking.Text = infos.Ranking != 0 ? infos.Ranking.ToString() : "NC";

            tb_single.Text = infos.SingleWin.ToString() + "|" + infos.SingleLose.ToString() + "|" + infos.SingleDraw.ToString();
            tb_match.Text = infos.MatchWin.ToString() + "|" + infos.MatchLose.ToString() + "|" + infos.MatchDraw.ToString();
            tb_tag.Text = infos.TagWin.ToString() + "|" + infos.TagLose.ToString() + "|" + infos.TagDraw.ToString();
            tb_ragequit.Text = infos.RageQuit.ToString();
            tb_giveup.Text = infos.GiveUp.ToString();

            tb_title.Text = infos.Title;
            if (infos.TeamID != 0)
            {
                tb_rank.Visibility = Visibility.Hidden;
                ImageBrush background = new ImageBrush(FormExecution.AssetsManager.GetTeamEmblem(infos.TeamID, infos.TeamEmblem));
                background.Stretch = Stretch.UniformToFill;
                team_emblem.Background = background;
                team_emblem.MouseLeftButtonDown += (sender, e) => Img_border_MouseLeftButtonDown(sender, e, infos.TeamID);
                this.tb_teamname.Text = infos.TeamName;
                team_emblem.MouseEnter += Team_emblem_MouseEnter;
                team_emblem.MouseLeave += Team_emblem_MouseLeave; ;
            }
            else
            {
                team_border.Visibility = Visibility.Hidden;
                Grid.SetColumnSpan(player_infos, 2);
                Grid.SetColumn(player_infos, 0);
            }

            this.Show();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => this.Activate()));
        }

        private void Team_emblem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!team_emblem.IsMouseOver && !popup_teamname.IsMouseOver)
                popup_teamname.IsOpen = false;
        }

        private void Team_emblem_MouseEnter(object sender, MouseEventArgs e)
        {
            popup_teamname.IsOpen = true;
        }

        private void Img_border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e, int teamID)
        {
            _admin.SendAskTeamProfile(teamID);
        }

        private BitmapImage GetImage(string path)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(path);
                image.EndInit();
                return image;
            }
            catch
            {
                return null;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.UpdateProfil -= _admin_UpdateProfil;
        }

        private void AvatarImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsMine())
                return;
            _admin.OpenAvatarsForm(this);
        }
        private void img_border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsMine())
                return;
            _admin.OpenBordersForm(this);
        }
        private void img_sleeve_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsMine())
                return;
            _admin.OpenSleevesForm(this);
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.ActivateChat();
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(20);
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

        private void tb_title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsMine())
                return;
            _admin.OpenTitlesForm(this);
        }

        private void tb_historic_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_userID != -1)
                _admin.SendAskGamesHistory(_userID);
        }
    }
}