using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromServer;
using hub_client.Assets;
using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
        private ProfilAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        AssetsManager PicsManager = new AssetsManager();

        public Profil(ProfilAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;

            _admin.UpdateProfil += _admin_UpdateProfil;

            this.FontFamily = FormExecution.AppDesignConfig.Font;

            this.MouseDown += Window_MouseDown;
        }

        private void _admin_UpdateProfil(StandardServerProfilInfo infos)
        {
            if (!infos.Avatar.IsHost)
                AvatarImg.Source = PicsManager.GetImage("Avatars", infos.Avatar.Id.ToString());
            else
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(
                        new System.Uri(infos.Avatar.URL),
                        Path.Combine(FormExecution.path, "Assets", "Avatars", "temp.png")
                        );
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

            tb_single.Text = infos.SingleWin.ToString() + "|" + infos.SingleLose.ToString() + "|" + infos.SingleDraw.ToString();
            tb_match.Text = infos.MatchWin.ToString() + "|" + infos.MatchLose.ToString() + "|" + infos.MatchDraw.ToString();
            tb_tag.Text = infos.TagWin.ToString() + "|" + infos.TagLose.ToString() + "|" + infos.TagDraw.ToString();
            tb_ragequit.Text = infos.RageQuit.ToString();
            tb_giveup.Text = infos.GiveUp.ToString();

            tb_title.Text = infos.Title;

            this.Show();
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            AvatarImg.Source = new BitmapImage(new Uri(Path.Combine(FormExecution.path, "Assets", "Avatars", "temp.png")));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.UpdateProfil -= _admin_UpdateProfil;
        }

        private void AvatarImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.OpenAvatarsForm();
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
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void tb_title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.OpenTitlesForm();
        }
    }
}
