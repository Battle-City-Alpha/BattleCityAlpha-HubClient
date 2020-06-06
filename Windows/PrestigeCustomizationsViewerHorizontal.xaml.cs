using BCA.Common;
using BCA.Common.Enums;
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
    /// Logique d'interaction pour PrestigeCustomizationsViewer.xaml
    /// </summary>
    public partial class PrestigeCustomizationsViewerHorizontal : Window
    {
        private PrestigeCustomizationsViewerAdministrator _admin;
        private Customization[] _customs;
        private bool _prestige;
        public PrestigeCustomizationsViewerHorizontal(PrestigeCustomizationsViewerAdministrator admin, bool prestige)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _admin = admin;
            _prestige = prestige;

            _admin.LoadPrestigeCustomizations += LoadPrestigeCustomizations;

            this.MouseDown += Window_MouseDown;
            this.Closed += PrestigeCustomizationsViewer_Closed;
            LoadStyle();

            this.btn_unlock.MouseLeftButtonDown += Btn_unlock_MouseLeftButtonDown;
        }

        private void Btn_unlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_customs[0].CustomizationType == CustomizationType.Avatar)
                System.Diagnostics.Process.Start("https://battlecityalpha.xyz/customizations/avatars.php");
            else if (_customs[0].CustomizationType == CustomizationType.Sleeve)
                System.Diagnostics.Process.Start("https://battlecityalpha.xyz/customizations/sleeves.php");
            else if (_customs[0].CustomizationType == CustomizationType.Partner)
                System.Diagnostics.Process.Start("https://battlecityalpha.xyz/customizations/partners.php");
        }

        private void PrestigeCustomizationsViewer_Closed(object sender, EventArgs e)
        {
            _admin.LoadPrestigeCustomizations -= LoadPrestigeCustomizations;
        }

        private void LoadPrestigeCustomizations(Customization[] customs)
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
            this.viewer_customs.LeftArrow();
        }

        private void img_down_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.viewer_customs.RightArrow();
        }

        private void btn_choose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = this.viewer_customs.GetIndex();
            Customization custom = _customs[index];
            if (_prestige)
                _admin.SendBuyPrestigeCustom(custom);
            else
            {
                switch (custom.CustomizationType)
                {
                    case CustomizationType.Avatar:
                        _admin.ChangeAvatar(custom.Id);
                        break;
                    case CustomizationType.Sleeve:
                        _admin.ChangeSleeve(custom.Id);
                        break;
                    case CustomizationType.Partner:
                        _admin.ChangePartner(custom.Id);
                        break;
                }
            }

            Close();
        }
    }
}
