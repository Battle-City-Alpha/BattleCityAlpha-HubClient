using BCA.Common;
using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour MonthPackViewer.xaml
    /// </summary>
    public partial class MonthPackViewer : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private MonthPackViewerAdministrator _admin;

        public MonthPackViewer(MonthPackViewerAdministrator admin, int avatar, int border, int sleeve)
        {
            InitializeComponent();
            LoadStyle();

            _admin = admin;

            this.MouseDown += Window_MouseDown;

            img_avatar.Source = FormExecution.AssetsManager.GetCustom(new Customization(CustomizationType.Avatar, avatar, false, ""));
            img_border.Source = FormExecution.AssetsManager.GetCustom(new Customization(CustomizationType.Border, border, false, ""));
            img_sleeve.Source = FormExecution.AssetsManager.GetCustom(new Customization(CustomizationType.Sleeve, sleeve, false, ""));

            btn_buy.MouseLeftButtonDown += Btn_buy_MouseLeftButtonDown;
        }

        private void Btn_buy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.SendBuyMonthPack();
            Close();
        }

        private void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;
            this.FontFamily = style.Font;

            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_buy });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1PrestigeShopButton");
                btn.Color2 = style.GetGameColor("Color2PrestigeShopButton");
                btn.Update();
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
                this.bg_border.CornerRadius = new CornerRadius(50);
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
