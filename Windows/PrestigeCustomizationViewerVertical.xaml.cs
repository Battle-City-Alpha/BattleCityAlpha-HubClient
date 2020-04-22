using BCA.Common;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour PrestigeCustomizationViewerVertical.xaml
    /// </summary>
    public partial class PrestigeCustomizationViewerVertical : Window
    {
        private PrestigeCustomizationsViewerAdministrator _admin;
        private Customization[] _customs;
        private bool _prestige;
        public PrestigeCustomizationViewerVertical(PrestigeCustomizationsViewerAdministrator admin, bool prestige)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            LoadStyle();
            _admin = admin;
            _prestige = prestige;

            _admin.LoadPrestigeCustomizations += _admin_LoadPrestigeCustomizations;

            this.MouseDown += Window_MouseDown;
            this.Closed += PrestigeCustomizationViewerVertical_Closed;
        }

        private void PrestigeCustomizationViewerVertical_Closed(object sender, EventArgs e)
        {
            _admin.LoadPrestigeCustomizations -= _admin_LoadPrestigeCustomizations;
        }

        private void _admin_LoadPrestigeCustomizations(Customization[] customs)
        {
            _customs = customs;
            this.viewer_customs.LoadFirstCustoms(customs);
        }

        private void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;
            this.FontFamily = style.Font;

            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.Add(btn_choose);

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1PrestigeShopButton");
                btn.Color2 = style.GetGameColor("Color2PrestigeShopButton");
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


        private void img_up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.viewer_customs.UpArrow();
        }

        private void img_down_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.viewer_customs.DownArrow();
        }

        private void btn_choose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = this.viewer_customs.GetIndex();
            Customization custom = _customs[index];

            if (_prestige)
                _admin.SendBuyPrestigeCustom(custom);
            else
                _admin.ChangeBorder(custom.Id);
            Close();
        }
    }
}
