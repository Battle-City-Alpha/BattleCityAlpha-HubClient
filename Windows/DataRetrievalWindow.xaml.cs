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
    /// Logique d'interaction pour DataRetrievalWindow.xaml
    /// </summary>
    public partial class DataRetrievalWindow : Window
    {
        private DataRetrievalAdministrator _admin;
        public DataRetrievalWindow(DataRetrievalAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            LoadStyle();

            this.MouseDown += Window_MouseDown;

            _admin = admin;
            _admin.StartLoading += _admin_StartLoading;
            _admin.EndLoading += _admin_EndLoading;

            btnRetrieval.MouseLeftButtonDown += LaunchRetrieval;
        }

        private void _admin_EndLoading()
        {
            progressBar_update.IsIndeterminate = false;
            btnRetrieval.ButtonText = "Lancer la récupération";
            btnRetrieval.Update();
            btnRetrieval.IsEnabled = true;
        }

        private void _admin_StartLoading()
        {
            progressBar_update.IsIndeterminate = true;
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btnRetrieval });

            AppDesignConfig style = FormExecution.AppDesignConfig;

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1DataRetrievalButton");
                btn.Color2 = style.GetGameColor("Color2DataRetrievalButton");
                btn.Update();
            }
            this.FontFamily = style.Font;
        }

        private void LaunchRetrieval(object sender, MouseButtonEventArgs e)
        {
            if (String.IsNullOrEmpty(tbUsername.Text) || String.IsNullOrEmpty(pbPassword.Password))
                return;

            _admin.SendAskDataRetrieval(tbUsername.Text, pbPassword.Password);
            btnRetrieval.ButtonText = "Récupération en cours... Ne pas quitter cette fenêtre.";
            btnRetrieval.Update();
            btnRetrieval.IsEnabled = false;
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
