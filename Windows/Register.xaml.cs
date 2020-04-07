using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.WindowsAdministrator;
using NLog;
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
    /// Logique d'interaction pour Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        RegisterAdminstrator _admin;

        public Register(RegisterAdminstrator admin)
        {
            InitializeComponent();
            _admin = admin;

            _admin.RegistrationComplete += _admin_RegistrationComplete;

            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }

        private void _admin_RegistrationComplete()
        {
            Close();
        }

        private void BCA_Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string username = tbUsername.Text;
            string password = pbPassword.Password;
            string passwordConfirm = pbPasswordConfirm.Password;
            string email = tbEmail.Text;
            string HID = FormExecution.HID;

            string encryptKey = File.ReadAllText("rsa_publickey.xml");

            if (password == passwordConfirm)
                _admin.Client.Send(PacketType.Register, new StandardClientRegister
                {
                    Username = username,
                    Password = CryptoManager.Encryption(password.Trim(), encryptKey),
                    Email = email,
                    HID = HID
                });
            else
                _admin.Client.OpenPopBox("Les mots de passe ne sont pas identiques.", "Problème");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.RegistrationComplete -= _admin_RegistrationComplete;
        }
    }
}
