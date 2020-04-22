using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour TitlesHandle.xaml
    /// </summary>
    public partial class TitlesHandle : Window
    {
        private TitlesHandleAdministrator _admin;
        private bool _prestigetitle;

        private AppDesignConfig style = FormExecution.AppDesignConfig;
        public TitlesHandle(TitlesHandleAdministrator admin, bool prestigetitle)
        {
            InitializeComponent();
            LoadStyle();
            this.FontFamily = style.Font;

            this.MouseDown += Window_MouseDown;
            this.Closed += TitlesHandle_Closed;

            _admin = admin;
            _admin.LoadTitles += _admin_LoadTitles;

            _prestigetitle = prestigetitle;
        }

        private void TitlesHandle_Closed(object sender, EventArgs e)
        {
            _admin.LoadTitles -= _admin_LoadTitles;
        }

        private void _admin_LoadTitles(List<string> titles)
        {
            cb_titles.ItemsSource = titles.ToArray();
            cb_titles.SelectedIndex = 0;
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_save_title });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1ToolsButton");
                btn.Color2 = style.GetGameColor("Color2ToolsButton");
                btn.Update();
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

        private void btn_save_title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_prestigetitle)
                _admin.ChangeTitle(cb_titles.SelectedItem.ToString());
            else
                _admin.BuyPrestigeTitle(cb_titles.SelectedItem.ToString());
            Close();
        }
    }
}
