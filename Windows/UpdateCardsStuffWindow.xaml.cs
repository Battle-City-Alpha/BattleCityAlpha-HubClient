using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour UpdateCardsStuffWindow.xaml
    /// </summary>
    public partial class UpdateCardsStuffWindow : Window
    {
        private string[] _updates;
        private bool _isDownloadFinished = false;
        public UpdateCardsStuffWindow(string[] updates, bool infini = false)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.progressBar_update.Value = 0;

            if (!infini)
            {
                _updates = updates;
                this.Loaded += UpdateCardsStuffWindow_Loaded;
            }
            else
            {
                this.tb_update.Visibility = Visibility.Hidden;
                this.tb_maj.Text = "Chargement...";
                //this.progressBar_update.IsIndeterminate = true;
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
        }

        public void SetProgressValue(double progress)
        {
            this.tb_maj.Text = "Chargement... " + (int)progress + "%";
            this.progressBar_update.Value = progress;

            if (progress == 100)
                _isDownloadFinished = true;
        }

        private void UpdateCardsStuffWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DownloadUpdates();
        }

        private void DownloadUpdates()
        {
            int i = 0;
            int n = _updates.Length;
            using (WebClient wc = new WebClient())
            {
                i++;
                tb_update.Text = i + "/" + n;
                for (int u = _updates.Length - 1; u >= 0; u--)
                {
                    UnzipFromStream(wc.OpenRead(GetUpdateFileLink(_updates[u])), FormExecution.path);
                }
            }

            progressBar_update.Value = 100;
            progressBar_update.IsIndeterminate = false;
            FormExecution.Client_PopMessageBox("Mise à jour terminée !", "Mise à jour", true);
            _isDownloadFinished = true;
            Close();
        }

        private Uri GetUpdateFileLink(string updatename)
        {
            return new Uri("http://battlecityalpha.xyz/BCA/UPDATEV2/CardsStuff/zip/" + updatename + ".zip");
        }
        private void UnzipFromStream(Stream zipStream, string outFolder)
        {
            progressBar_update.IsIndeterminate = true;
            using (var zipInputStream = new ZipInputStream(zipStream))
            {
                while (zipInputStream.GetNextEntry() is ZipEntry zipEntry)
                {
                    var entryFileName = zipEntry.Name;
                    // To remove the folder from the entry:
                    //var entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here
                    // to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    // 4K is optimum
                    var buffer = new byte[4096];

                    // Manipulate the output filename here as desired.
                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Skip directory entry
                    if (Path.GetFileName(fullZipToPath).Length == 0)
                    {
                        continue;
                    }

                    // Unzip file in buffered chunks. This is just as fast as unpacking
                    // to a buffer the full size of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                    }
                }
            }
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
