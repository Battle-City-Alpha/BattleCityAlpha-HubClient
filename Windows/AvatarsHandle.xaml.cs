using BCA.Common;
using hub_client.Assets;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour AvatarsHandle.xaml
    /// </summary>
    public partial class AvatarsHandle : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        AssetsManager PicsManager = new AssetsManager();

        private AvatarsHandleAdministrator _admin;
        private Customization[] _avatars;

        public AvatarsHandle(AvatarsHandleAdministrator admin)
        {
            InitializeComponent();
            LoadStyle();
            this.FontFamily = style.Font;

            _admin = admin;

            _admin.LoadAvatars += _admin_LoadAvatars;
            control_avatar.cb_avatar.SelectionChanged += cb_avatar_SelectionChanged;
            control_avatar.btn_save_avatar.MouseLeftButtonDown += btn_save_avatar_MouseLeftButtonDown;

            this.MouseDown += Window_MouseDown;
        }
        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { control_avatar.btn_save_avatar });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1ToolsButton");
                btn.Color2 = style.GetGameColor("Color2ToolsButton");
                btn.Update();
            }
        }

        private void _admin_LoadAvatars(Customization[] avatars)
        {
            _avatars = avatars;
            control_avatar.cb_avatar.Items.Clear();
            foreach (Customization avatar in avatars)
                control_avatar.cb_avatar.Items.Add(avatar.Id);
        }

        private void cb_avatar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = control_avatar.cb_avatar.SelectedIndex;
            Customization avatar = _avatars[index];
            if (!avatar.IsHost)
            {
                int avatarId = Convert.ToInt32(control_avatar.cb_avatar.SelectedItem);
                control_avatar.AvatarImg.Source = PicsManager.GetImage("Avatars", avatarId.ToString());
            }
            else
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(
                        new System.Uri(avatar.URL),
                        Path.Combine(FormExecution.path, "Assets", "Avatars", "temp.png")
                        );
                }
            }
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            control_avatar.AvatarImg.Source = new BitmapImage(new Uri(Path.Combine(FormExecution.path, "Assets", "Avatars", "temp.png")));
        }

        private void btn_save_avatar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.ChangeAvatar(Convert.ToInt32(control_avatar.cb_avatar.SelectedValue.ToString()));
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.LoadAvatars -= _admin_LoadAvatars;
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
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
