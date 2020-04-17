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
    /// Logique d'interaction pour SleevesHandle.xaml
    /// </summary>
    public partial class SleevesHandle : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        AssetsManager PicsManager = new AssetsManager();

        private SleevesHandleAdministrator _admin;
        private Customization[] _sleeves;

        public SleevesHandle(SleevesHandleAdministrator admin)
        {
            InitializeComponent();
            LoadStyle();
            this.FontFamily = style.Font;

            _admin = admin;

            _admin.LoadSleeves += _admin_LoadSleeves;
            cb_sleeves.SelectionChanged += Cb_sleeves_SelectionChanged;
            btn_save_sleeve.MouseLeftButtonDown += Btn_save_sleeve_MouseLeftButtonDown;

            this.MouseDown += Window_MouseDown;
        }

        private void Btn_save_sleeve_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.ChangeSleeve(Convert.ToInt32(cb_sleeves.SelectedValue.ToString()));
            Close();
        }

        private void Cb_sleeves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = cb_sleeves.SelectedIndex;
            Customization sleeve = _sleeves[index];
            if (!sleeve.IsHost)
            {
                int sleeveId = Convert.ToInt32(cb_sleeves.SelectedItem);
                img_sleeve.Source = PicsManager.GetImage("Sleeves", sleeveId.ToString());
            }
            else
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(
                        new System.Uri(sleeve.URL),
                        Path.Combine(FormExecution.path, "Assets", "Sleeves", "temp.png")
                        );
                }
            }
        }
        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            img_sleeve.Source = new BitmapImage(new Uri(Path.Combine(FormExecution.path, "Assets", "Sleeves", "temp.png")));
        }

        private void _admin_LoadSleeves(Customization[] sleeves)
        {
            _sleeves = sleeves;
            cb_sleeves.Items.Clear();
            foreach (Customization sleeve in _sleeves)
                cb_sleeves.Items.Add(sleeve.Id);
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_save_sleeve });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1ToolsButton");
                btn.Color2 = style.GetGameColor("Color2ToolsButton");
                btn.Update();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.LoadSleeves -= _admin_LoadSleeves;
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
