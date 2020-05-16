using BCA.Common.Enums;
using NLog;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour CustomViewer.xaml
    /// </summary>
    public partial class CustomViewer : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public CustomViewer(CustomizationType ctype, string text, int id)
        {
            InitializeComponent();

            this.MouseDown += Window_MouseDown;

            AdaptImageSize(ctype, id);
            tb_text.Text = text;
        }
        private void AdaptImageSize(CustomizationType ctype, int id)
        {
            switch (ctype)
            {
                case CustomizationType.Avatar:
                    img_custom.Width = 256;
                    img_custom.Height = 256;
                    img_custom.Source = GetImage("Avatars", id.ToString());
                    break;
                case CustomizationType.Sleeve:
                    img_custom.Width = 177;
                    img_custom.Height = 254;
                    img_custom.Source = GetImage("Sleeves", id.ToString());
                    break;
                case CustomizationType.Border:
                    img_custom.Width = 306;
                    img_custom.Height = 136;
                    img_custom.Source = GetImage("Borders", id.ToString());
                    break;
            }
        }
        private BitmapImage GetImage(string directory, string img)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(System.IO.Path.Combine(FormExecution.path, "Assets", directory, img + ".png"));
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
                logger.Error("IMAGE LOADING - {0}", ex);
                return null;
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
