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
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        LoginAdminstrator _admin;

        public Login(LoginAdminstrator admin)
        {
            InitializeComponent();
            _admin = admin;
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
    }
}
