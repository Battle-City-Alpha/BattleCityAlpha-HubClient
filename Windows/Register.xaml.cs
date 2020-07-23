using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _admin = admin;

            _admin.RegistrationComplete += _admin_RegistrationComplete;

            this.FontFamily = FormExecution.AppDesignConfig.Font;
            this.MouseDown += Window_MouseDown;

            this.Loaded += Register_Loaded;
        }

        private void Register_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.RenderTransform = scale;

            DoubleAnimation growAnimationClose = new DoubleAnimation();
            growAnimationClose.Duration = TimeSpan.FromMilliseconds(100);
            growAnimationClose.From = 0.0;
            growAnimationClose.To = 1.0;
            storyboard.Children.Add(growAnimationClose);

            Storyboard.SetTargetProperty(growAnimationClose, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimationClose, this);

            storyboard.Begin();
        }

        private void _admin_RegistrationComplete()
        {
            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.RenderTransform = scale;

            DoubleAnimation growAnimationClose = new DoubleAnimation();
            growAnimationClose.Duration = TimeSpan.FromMilliseconds(100);
            growAnimationClose.From = 1.0;
            growAnimationClose.To = 0.0;
            storyboard.Children.Add(growAnimationClose);

            Storyboard.SetTargetProperty(growAnimationClose, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimationClose, this);

            storyboard.Completed += Storyboard_Completed;

            storyboard.Begin();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
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
            {
                if (!_admin.Client.IsConnected)
                {
                    FormExecution.StartConnexion();
                    _admin.Client.Connected += () => Client_Connected(username, password, encryptKey, email, HID);
                }
                else
                    Client_Connected(username, password, encryptKey, email, HID);
            }
            else
                _admin.Client.OpenPopBox("Les mots de passe ne sont pas identiques.", "Problème");
        }

        private void Client_Connected(string username, string password, string encryptKey, string email, string HID)
        {
            _admin.Client.Send(PacketType.Register, new StandardClientRegister
            {
                Username = username,
                Password = CryptoManager.Encryption(password.Trim(), encryptKey),
                Email = email,
                HID = HID
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.RegistrationComplete -= _admin_RegistrationComplete;
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.RenderTransform = scale;

            DoubleAnimation growAnimationClose = new DoubleAnimation();
            growAnimationClose.Duration = TimeSpan.FromMilliseconds(100);
            growAnimationClose.From = 1.0;
            growAnimationClose.To = 0.0;
            storyboard.Children.Add(growAnimationClose);

            Storyboard.SetTargetProperty(growAnimationClose, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimationClose, this);

            storyboard.Completed += Storyboard_Completed;

            storyboard.Begin();
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
