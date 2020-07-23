using BCA.Common;
using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour CustomSpecialPack.xaml
    /// </summary>
    public partial class CustomSpecialPackWindow : Window
    {
        CustomSpecialPack _pack;
        TimeSpan _interval;
        public event Action PurchaseBtnClick;
        public CustomSpecialPackWindow(CustomSpecialPack pack)
        {
            InitializeComponent();
            LoadStyle();

            _pack = pack;
            LoadCustom();

            _interval = (pack.EndTime - DateTime.Now);
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.IsEnabled = true;
            timer.Tick += Timer_Tick;
            lbl_time.Content = string.Format("{0}h{1}m{2}s", _interval.Hours + 24 * _interval.Days, _interval.Minutes, _interval.Seconds);

            this.btn_buy.MouseLeftButtonDown += Btn_buy_MouseLeftButtonDown;

            this.MouseDown += Window_MouseDown;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _interval = _interval.Add(TimeSpan.FromSeconds(-1));
            lbl_time.Content = string.Format("{0}h{1}m{2}s", _interval.Hours + 24 * _interval.Days, _interval.Minutes, _interval.Seconds);
        }

        private void Btn_buy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PurchaseBtnClick?.Invoke();
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
        private void LoadCustom()
        {
            foreach (Customization custom in _pack.Customs)
            {
                Border bd = new Border();
                bd.Background = new SolidColorBrush(Colors.White);
                bd.Background.Opacity = 0.8;
                bd.Padding = new Thickness(5);
                bd.CornerRadius = new CornerRadius(5);
                bd.Margin = new Thickness(5, 0, 5, 0);
                bd.HorizontalAlignment = HorizontalAlignment.Center;
                bd.VerticalAlignment = VerticalAlignment.Center;

                Image img = new Image();
                img.Source = FormExecution.AssetsManager.GetCustom(custom);

                switch (custom.CustomizationType)
                {
                    case CustomizationType.Avatar:
                    case CustomizationType.Partner:
                        img.Width = 256;
                        img.Height = 256;
                        break;
                    case CustomizationType.Sleeve:
                        img.Width = 177;
                        img.Height = 254;
                        break;
                    case CustomizationType.Border:
                        img.Width = 306;
                        img.Height = 136;
                        break;
                }

                bd.Child = img;

                wp_customs.Children.Add(bd);
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
