using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Standard.FromServer;
using hub_client.Assets;
using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
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

        AssetsManager PicsManager = new AssetsManager();

        private bool IsMine()
        {
            return FormExecution.Username == this.tb_username.Text;
        }

        public Profil(ProfilAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _admin = admin;

            _admin.UpdateProfil += _admin_UpdateProfil;

            this.FontFamily = FormExecution.AppDesignConfig.Font;

            this.MouseDown += Window_MouseDown;
        }

        private void _admin_UpdateProfil(StandardServerProfilInfo infos)
        {
            if (!infos.Avatar.IsHost)
                img_avatar.Source = PicsManager.GetImage("Avatars", infos.Avatar.Id.ToString());
            else
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, infos.Avatar);
                        wc.DownloadFileAsync(
                            new System.Uri(infos.Avatar.URL),
                            Path.Combine(FormExecution.path, "Assets", "Avatars", "temp.png")
                            );
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur", true);
                }
                
            }

            if (!infos.Border.IsHost)
                img_border.Source = PicsManager.GetImage("Borders", infos.Border.Id.ToString());
            else
            {
                try { 
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, infos.Border);
                    wc.DownloadFileAsync(
                        new System.Uri(infos.Border.URL),
                        Path.Combine(FormExecution.path, "Assets", "Borders", "temp.png")
                        );
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur", true);
                }
            }

            if (!infos.Sleeve.IsHost)
                img_sleeve.Source = PicsManager.GetImage("Sleeves", infos.Sleeve.Id.ToString());
            else
            {
                try { 
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, infos.Sleeve);
                    wc.DownloadFileAsync(
                        new System.Uri(infos.Sleeve.URL),
                        Path.Combine(FormExecution.path, "Assets", "Sleeves", "temp.png")
                        );
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur", true);
                }
            }

            tb_username.Text = infos.Username;
            tb_cardnumber.Text = infos.CardNumber.ToString();
            tb_level.Text = infos.Level.ToString();
            tb_experience.Text = infos.Exp.ToString();

            tb_rankedwin.Text = infos.RankedWin.ToString();
            tb_rankedlose.Text = infos.RankedLose.ToString();
            tb_rankeddraw.Text = infos.RankedDraw.ToString();
            tb_elo.Text = infos.ELO.ToString();
            tb_rank.Text = infos.Rank.ToString();
            tb_ranking.Text = infos.Ranking.ToString();

            tb_single.Text = infos.SingleWin.ToString() + "|" + infos.SingleLose.ToString() + "|" + infos.SingleDraw.ToString();
            tb_match.Text = infos.MatchWin.ToString() + "|" + infos.MatchLose.ToString() + "|" + infos.MatchDraw.ToString();
            tb_tag.Text = infos.TagWin.ToString() + "|" + infos.TagLose.ToString() + "|" + infos.TagDraw.ToString();
            tb_ragequit.Text = infos.RageQuit.ToString();
            tb_giveup.Text = infos.GiveUp.ToString();

            tb_title.Text = infos.Title;

            this.Show();
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e, Customization item)
        {
            switch (item.CustomizationType)
            {
                case CustomizationType.Avatar:
                    img_avatar.Source = GetImage(Path.Combine(FormExecution.path, "Assets", "Avatars", "temp.png"));
                    break;
                case CustomizationType.Border:
                    img_border.Source = GetImage(Path.Combine(FormExecution.path, "Assets", "Borders", "temp.png"));
                    break;
                case CustomizationType.Sleeve:
                    img_sleeve.Source = GetImage(Path.Combine(FormExecution.path, "Assets", "Sleeves", "temp.png"));
                    break;
            }
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
    }
}
