using BCA.Common;
using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Tulpep.NotificationWindow;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour PrivateMessage.xaml
    /// </summary>
    public partial class PrivateMessage : Window
    {
        [DllImport("user32")] public static extern int FlashWindow(IntPtr hwnd, bool bInvert);

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        private PlayerInfo _target;
        PrivateMessageAdministrator _admin;

        public PrivateMessage(string target, PrivateMessageAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _admin = admin;
            _target = _admin.Client.GetPlayerInfo(target);
            if (_target == null)
            {
                Close();
                return;
            }
            Title = "Privé : " + _target.Username;
            tb_username.Text = _target.Username;

            _admin.MessageRecieved += _admin_MessageRecieved;

            this.FontFamily = FormExecution.AppDesignConfig.Font;

            this.MouseDown += Window_MouseDown;

            this.Activated += PrivateMessage_Activated;

            this.rtbChat.RefreshStyle();

            tbChat.Focus();
            tbChat.tbChat.Select(0, 0);
        }

        private void PrivateMessage_Activated(object sender, EventArgs e)
        {
            tbChat.Focus();
            tbChat.tbChat.Select(0, 0);
        }

        private void _admin_MessageRecieved(PlayerInfo infos, string message)
        {
            Dispatcher.InvokeAsync(delegate { rtbChat.OnPlayerColoredMessage(FormExecution.AppDesignConfig.GetGameColor("StandardMessageColor"), infos, message, true); });
            Dispatcher.InvokeAsync(delegate { Show(); });

            if (!this.IsActive)
            {
                if (FormExecution.ClientConfig.PMPopup)
                {
                    PopupNotifier popup = new PopupNotifier();
                    popup.BodyColor = style.GetDrawingColor(style.GetGameColor("PopupPMBackgroundColor"));
                    popup.ContentColor = style.GetDrawingColor(style.GetGameColor("PopupPMContentColor"));
                    popup.HeaderColor = style.GetDrawingColor(style.GetGameColor("PopupPMHeaderColor"));
                    popup.TitleColor = style.GetDrawingColor(style.GetGameColor("PopupPMTitleColor"));
                    popup.BorderColor = System.Drawing.Color.White;
                    popup.TitleText = "Battle City Alpha - MP : " + infos.Username;
                    popup.ContentText = message;

                    try
                    {
                        if (infos.Avatar.IsHost)
                        {
                            popup.Image = Image.FromFile(FormExecution.AssetsManager.GetSource("Avatars", "A_" + infos.UserId + ".png"));
                        }
                        else
                        {
                            popup.Image = Image.FromFile(FormExecution.AssetsManager.GetSource("Avatars", infos.Avatar.Id + ".png"));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        popup.Image = Image.FromFile(FormExecution.AssetsManager.GetSource("Logo", "pm_logo.png"));
                    }
                    popup.ImageSize = new System.Drawing.Size(popup.Size.Height - 18, popup.Size.Height - 18);
                    popup.ImagePadding = new System.Windows.Forms.Padding(5);

                    popup.Popup();
                }

                WindowInteropHelper wih = new WindowInteropHelper(this);
                FlashWindow(wih.Handle, true);
            }
        }

        private void BCA_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (tbChat.GetText() == string.Empty)
                        return;
                    _admin.SendMessage(_target, tbChat.GetText());
                    rtbChat.OnSpecialColoredMessage(FormExecution.AppDesignConfig.GetGameColor("StandardMessageColor"), "[" + FormExecution.Username + "]: " + tbChat.GetText());
                    tbChat.Clear();
                    break;
            }
            e.Handled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.MessageRecieved -= _admin_MessageRecieved;
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
                this.bg_border.CornerRadius = new CornerRadius(20);
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
