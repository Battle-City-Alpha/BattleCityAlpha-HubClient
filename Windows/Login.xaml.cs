using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        LoginAdminstrator _admin;

        public Login(LoginAdminstrator admin)
        {
            InitializeComponent();
            _admin = admin;

            _admin.LoginComplete += _admin_LoginComplete;
            Loaded += Login_Loaded;
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            cbRememberMe.IsChecked = FormExecution.AppConfig.RememberMe;
            if (cbRememberMe.IsChecked == true)
            {
                tbUsername.Text = FormExecution.AppConfig.Username;
                pbPassword.Password = FormExecution.AppConfig.Password;
            }
        }

        private void _admin_LoginComplete()
        {
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnRegister.BorderThickness = new Thickness(2);
            FormExecution.OpenRegisterForm();
        }

        private void btnRegister_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            btnRegister.BorderThickness = new Thickness(1);
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
                FormExecution.AppConfig.RememberMe = cbRememberMe.IsChecked.Value;
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
        }
    }
}
