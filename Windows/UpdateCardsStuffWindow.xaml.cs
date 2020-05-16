using System;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour UpdateCardsStuffWindow.xaml
    /// </summary>
    public partial class UpdateCardsStuffWindow : Window
    {
        public bool _isDownloadFinished = false;
        public UpdateCardsStuffWindow(bool cdb)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.progressBar_update.Value = 0;

            if (!cdb)
            {
                progressBar_update.IsIndeterminate = true;
            }
            else
            {
                this.tb_update.Visibility = Visibility.Hidden;
                this.tb_maj.Text = "Chargement...";
            }

            this.MouseDown += Window_MouseDown;
            this.Closed += UpdateCardsStuffWindow_Closed;
        }

        private void UpdateCardsStuffWindow_Closed(object sender, EventArgs e)
        {
            if (!_isDownloadFinished)
                Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }

        public void EndDownload()
        {
            _isDownloadFinished = true;
            SetProgressValue(100.0);
        }

        public void SetProgressValue(double progress)
        {
            this.tb_maj.Text = "Chargement... " + (int)progress + "%";
            this.progressBar_update.Value = progress;

            if (progress == 100)
                _isDownloadFinished = true;
        }
        public void SetProgressUpdate(int i, int n)
        {
            this.tb_update.Text = i + "/" + n;
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
                this.bg_border.CornerRadius = new CornerRadius(40, 0, 40, 20);
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
