using hub_client.Assets;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Tools.xaml
    /// </summary>
    public partial class Tools : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        private ClientConfig client_config = FormExecution.ClientConfig;
        AssetsManager PicsManager = new AssetsManager();

        public Tools()
        {
            InitializeComponent();

            LoadStyle();

            cb_connectionmsg.IsChecked = client_config.Connexion_Message;
            cb_greet.IsChecked = client_config.Greet;
            cb_traderequest.IsChecked = client_config.Request;
            cb_duelrequest.IsChecked = client_config.Trade;

            cb_avatar.Items.Add("1");
            cb_avatar.Items.Add("12");
            cb_avatar.Items.Add("24");
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_color, btn_img, btn_save, btn_save_avatar });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.Color1ToolsButton;
                btn.Color2 = style.Color2ToolsButton;
                btn.Update();
            }
        }

        private void btn_save_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            client_config.Greet = (bool)cb_greet.IsChecked;
            client_config.Request = (bool)cb_duelrequest.IsChecked;
            client_config.Trade = (bool)cb_traderequest.IsChecked;
            client_config.Connexion_Message = (bool)cb_connectionmsg.IsChecked;

            FormExecution.Client.OpenPopBox("Configuration mise à jour.", "Configuration");
            Close();
        }

        private void btn_color_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(Path.Combine(FormExecution.path, "style.json"));
        }

        private void btn_img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", Path.Combine(FormExecution.path, "Assets", "Background"));
        }

        private void cb_avatar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int avatarId = Convert.ToInt32(cb_avatar.SelectedItem);
            AvatarImg.Source = PicsManager.GetImage("Avatars", avatarId.ToString("D2"));
        }
    }
}
