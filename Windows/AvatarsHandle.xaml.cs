using hub_client.Assets;
using hub_client.Configuration;
using hub_client.Windows.Controls;
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
    /// Logique d'interaction pour AvatarsHandle.xaml
    /// </summary>
    public partial class AvatarsHandle : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        AssetsManager PicsManager = new AssetsManager();

        private AvatarsHandleAdministrator _admin;

        public AvatarsHandle(AvatarsHandleAdministrator admin)
        {
            InitializeComponent();
            LoadStyle();

            _admin = admin;

            _admin.LoadAvatars += _admin_LoadAvatars;
            control_avatar.cb_avatar.SelectionChanged += cb_avatar_SelectionChanged;
            control_avatar.btn_save_avatar.MouseLeftButtonDown += btn_save_avatar_MouseLeftButtonDown;
        }
        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { control_avatar.btn_save_avatar });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.Color1ToolsButton;
                btn.Color2 = style.Color2ToolsButton;
                btn.Update();
            }
        }

        private void _admin_LoadAvatars(int[] avatars)
        {
            control_avatar.cb_avatar.ItemsSource = avatars;
        }

        private void cb_avatar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int avatarId = Convert.ToInt32(control_avatar.cb_avatar.SelectedItem);
            control_avatar.AvatarImg.Source = PicsManager.GetImage("Avatars", avatarId.ToString("D2"));
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
    }
}
