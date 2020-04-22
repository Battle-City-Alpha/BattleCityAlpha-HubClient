﻿using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        LoginAdminstrator _admin;
        bool _complete = false;

        public Login(LoginAdminstrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _admin = admin;

            _admin.LoginComplete += _admin_LoginComplete;
            Loaded += Login_Loaded;

            this.MouseDown += Login_MouseDown;
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btnPasswordForgotten, btnRegister, btnConnexion });

            AppDesignConfig style = FormExecution.AppDesignConfig;

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1LoginButton");
                btn.Color2 = style.GetGameColor("Color2LoginButton");
                btn.Update();
            }

            this.FontFamily = style.Font;
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            cbRememberMe.IsChecked = FormExecution.AppConfig.RememberMe;
            if (cbRememberMe.IsChecked == true)
            {
                tbUsername.Text = FormExecution.AppConfig.Username;
                pbPassword.Password = FormExecution.AppConfig.Password;
            }

            LoadStyle();

            using (WebClient wc = new WebClient())
            {
                this.rtb_patchnotes.AppendText(wc.DownloadString("http://battlecityalpha.xyz/BCA/UPDATEV2/Client/news.txt"));
            }            
        }

        private void _admin_LoginComplete()
        {
            _complete = true;
            Close();
        }

        private void BCA_Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tbUsername.Text == string.Empty || pbPassword.Password == string.Empty)
            {
                _admin.Client.OpenPopBox("Vous ne pouvez pas laisser de champs vide.", "Problème de connexion");
                return;
            }
            if (cbRememberMe.IsChecked == true)
            {
                FormExecution.AppConfig.Username = tbUsername.Text;
                FormExecution.AppConfig.Password = pbPassword.Password;
                FormExecution.AppConfig.RememberMe = true;
                FormExecution.AppConfig.Save();
            }
            string username = tbUsername.Text;
            string password = pbPassword.Password;
            string HID = FormExecution.HID;

            FormExecution.Username = username;

            string encryptKey = File.ReadAllText("rsa_publickey.xml");
            _admin.SendAuthentification(username, password, encryptKey, HID);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.LoginComplete -= _admin_LoginComplete;
            Loaded -= Login_Loaded;
            if (!_complete)
                Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }

        private void CbRememberMe_Checked(object sender, RoutedEventArgs e)
        {
            if (tbUsername.Text != "")
                FormExecution.AppConfig.Username = tbUsername.Text;
            if (pbPassword.Password != "")
                FormExecution.AppConfig.Password = pbPassword.Password;
            FormExecution.AppConfig.RememberMe = true;
            FormExecution.AppConfig.Save();
        }

        private void CbRememberMe_Unchecked(object sender, RoutedEventArgs e)
        {
            FormExecution.AppConfig.Username = "";
            FormExecution.AppConfig.Password = "";
            FormExecution.AppConfig.RememberMe = false;
            FormExecution.AppConfig.Save();
        }

        private void BtnRegister_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.OpenRegisterForm();
        }

        private void BtnPasswordForgotten_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://battlecityalpha.xyz/passwordtools/resetpassword.php");
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
                this.bg_border.CornerRadius = new CornerRadius(110, 0, 110, 40);
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
        private void Login_MouseDown(object sender, MouseButtonEventArgs e)
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
