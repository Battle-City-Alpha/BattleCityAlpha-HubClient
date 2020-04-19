using BCA.Common;
using hub_client.Assets;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
    /// Logique d'interaction pour BordersHandle.xaml
    /// </summary>
    public partial class BordersHandle : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        AssetsManager PicsManager = new AssetsManager();

        private BordersHandleAdministrator _admin;
        private Customization[] _borders;

        public BordersHandle(BordersHandleAdministrator admin)
        {
            InitializeComponent();
            LoadStyle();
            this.FontFamily = style.Font;

            _admin = admin;

            _admin.LoadBorders += _admin_LoadBorders;
            cb_borders.SelectionChanged += Cb_borders_SelectionChanged;
            btn_save_border.MouseLeftButtonDown += Btn_save_border_MouseLeftButtonDown;

            this.MouseDown += Window_MouseDown;
        }

        private void Btn_save_border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.ChangeBorder(Convert.ToInt32(cb_borders.SelectedValue.ToString()));
            Close();
        }

        private void Cb_borders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = cb_borders.SelectedIndex;
            Customization border = _borders[index];
            if (!border.IsHost)
            {
                int borderId = Convert.ToInt32(cb_borders.SelectedItem);
                img_border.Source = PicsManager.GetImage("Borders", borderId.ToString());
            }
            else
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(
                        new System.Uri(border.URL),
                        Path.Combine(FormExecution.path, "Assets", "Borders", "temp.png")
                        );
                }
            }
        }
        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            img_border.Source = new BitmapImage(new Uri(Path.Combine(FormExecution.path, "Assets", "Borders", "temp.png")));
        }

        private void _admin_LoadBorders(Customization[] borders)
        {
            _borders = borders;
            cb_borders.Items.Clear();
            foreach (Customization border in borders)
                cb_borders.Items.Add(border.Id);
            cb_borders.SelectedIndex = 0;
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_save_border });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1ToolsButton");
                btn.Color2 = style.GetGameColor("Color2ToolsButton");
                btn.Update();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.LoadBorders -= _admin_LoadBorders;
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
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}