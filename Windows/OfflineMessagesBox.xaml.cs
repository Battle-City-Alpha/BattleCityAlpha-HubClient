using BCA.Common;
using hub_client.Configuration;
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
    /// Logique d'interaction pour OfflineMessages.xaml
    /// </summary>
    public partial class OfflineMessagesBox : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        private OfflineMessage[] _messages;
        private int _currentIndex = 0;

        public OfflineMessagesBox()
        {
            InitializeComponent();

            this.MouseDown += Window_MouseDown;
        }

        public void LoadStyle()
        {
            this.FontFamily = style.Font;
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
                this.bg_border.CornerRadius = new CornerRadius(10, 10, 10, 200);
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

        public void LoadMessages(OfflineMessage[] msgs)
        {
            _messages = msgs;
            if (msgs.Count() == 1)
                boxTitle.Text = "Message reçu hors-ligne";

            UpdateMessage();
        }

        private void UpdateMessage()
        {
            currentMessage.Text = (_currentIndex + 1).ToString();
            totalMessages.Text = _messages.Count().ToString();

            rtb_inbox.SetText(FormatMessage(_messages[_currentIndex]));
        }

        private string FormatMessage(OfflineMessage message)
        {
            return "Expéditeur: " + message.Sender + Environment.NewLine + Environment.NewLine + "Message: " + message.Message;
        }

        private void img_up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentIndex >= _messages.Count() - 1)
                return;

            _currentIndex++;
            UpdateMessage();
        }

        private void img_down_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentIndex <= 0)
                return;

            _currentIndex--;
            UpdateMessage();
        }
    }
}
