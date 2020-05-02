using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour ChangePicsStyle.xaml
    /// </summary>
    public partial class ChangePicsStyle : Window
    {
        public ChangePicsStyle()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            LoadStyle();

            btnDownloadPics.MouseLeftButtonDown += StartDownloadingPics;

            this.Loaded += ChangePicsStyle_Loaded;
            this.MouseDown += Window_MouseDown;
        }

        private void ChangePicsStyle_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormExecution.ClientConfig.BCA_Card_Design)
                bca_design.IsChecked = true;
            else
                base_design.IsChecked = true;
        }

        private void StartDownloadingPics(object sender, MouseButtonEventArgs e)
        {
            btnDownloadPics.IsEnabled = false;
            //string[] pics = Directory.GetFiles(Path.Combine(FormExecution.path, "BattleCityAlpha", "pics"));
            progressBar_update.Maximum = CardManager.Count;
            tb_update_total.Text = CardManager.Count.ToString();

            DownloadPics();
        }

        private async void DownloadPics()
        {
            foreach (int card in CardManager.GetKeys())
            {
                string id = card.ToString();
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += PicDownloaded;
                    wc.DownloadFileAsync(
                        GetUri(id),
                        Path.Combine(FormExecution.path, "BattleCityAlpha", "pics", id + ".jpg")
                        );
                }
                await Task.Delay(100);
            }
        }
        private string ParseFileName(string filename)
        {
            return filename.Split('\\').Last().Split('.').First();
        }

        private void PicDownloaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            progressBar_update.Value++;
            tb_update_count.Text = progressBar_update.Value.ToString();

            if (tb_update_count.Text == tb_update_total.Text)
            {
                btnDownloadPics.IsEnabled = true;
                FormExecution.ClientConfig.BCA_Card_Design = bca_design.IsChecked == true;
                FormExecution.Client_PopMessageBox("Le changement de style de cartes est terminé et effectif !", "Changement style de carte", true);
                Close();
            }
        }

        public Uri GetUri(string id)
        {
            string s = "";
            if (base_design.IsChecked == true)
                s = string.Format("http://raw.githubusercontent.com/Tic-Tac-Toc/Pics_BCA/master/base_design/{0}.jpg", id);
            else
                s = string.Format("http://raw.githubusercontent.com/Tic-Tac-Toc/Pics_BCA/master/bca_design/{0}.jpg", id);

            return new Uri(s);
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btnDownloadPics });

            AppDesignConfig style = FormExecution.AppDesignConfig;

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1DataRetrievalButton");
                btn.Color2 = style.GetGameColor("Color2DataRetrievalButton");
                btn.Update();
            }
            this.FontFamily = style.Font;
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
