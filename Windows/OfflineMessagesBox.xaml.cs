using BCA.Common;
using hub_client.Cards;
using hub_client.Configuration;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

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
            ParseMsg();
            if (msgs.Count() == 1)
                boxTitle.Text = "Message reçu hors-ligne";

            UpdateMessage();
        }
        private void ParseMsg()
        {
            for (int i = 0; i < _messages.Length; i++)
            {
                if (_messages[i].Message.Contains("("))
                {
                    string[] parts = _messages[i].Message.Split('(');
                    string id = parts[1].Split(')')[0];
                    CardInfos c = CardManager.GetCard(Convert.ToInt32(id));
                    if (c == null)
                        continue;
                    _messages[i].Message = parts[0] + "(" + c.Name + parts[1].Substring(id.Length);
                }
            }
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
