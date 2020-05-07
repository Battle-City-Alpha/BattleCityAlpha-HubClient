using hub_client.Configuration;
using hub_client.Helpers;
using hub_client.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Blacklist.xaml
    /// </summary>
    public partial class Blacklist : Window
    {
        public BlacklistManager Manager;


        public Blacklist(BlacklistManager manager)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            Manager = manager;

            foreach (var player in Manager.Blacklist)
                lbBlacklist.Items.Add(player.Value);

            LoadStyle();

            this.MouseDown += Window_MouseDown;
        }

        private void btnRetire_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string username = lbBlacklist.SelectedItem.ToString();
                int userid = Manager.Blacklist.FirstOrDefault(x => x.Value == username).Key;
                lbBlacklist.SelectedItem = null;
                lbBlacklist.Items.Remove(username);
                Manager.Blacklist.Remove(userid);
                Manager.Save();
            }
            catch (Exception)
            {

            }
        }
        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btnRetire });

            AppDesignConfig style = FormExecution.AppDesignConfig;

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1ToolsButton");
                btn.Color2 = style.GetGameColor("Color2ToolsButton");
                btn.Update();
            }
            this.FontFamily = style.Font;
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.ActivateChat();
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
