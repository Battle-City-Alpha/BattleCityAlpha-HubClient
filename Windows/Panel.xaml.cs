using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logique d'interaction pour Panel.xaml
    /// </summary>
    public partial class Panel : Window
    {
        private PanelAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        private PlayerInfo[] profiles;
        private PlayerInfo profileselected;

        public Panel(PanelAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;
            _admin.UpdatePlayersList += _admin_UpdatePlayersList;
            _admin.UpdateProfile += _admin_UpdateProfile;

            _admin.Client.Send(PacketType.PanelUserlist, new StandardClientOpenPanel { });
            LoadStyle();
        }

        private void _admin_UpdateProfile(string[] accounts, string ip, string obs, int bp)
        {
            string username_accounts = "";
            foreach (string player in accounts)
                username_accounts += player + ",";
            lblAccounts.Text = username_accounts;
            lblIP.Text = ip;
            rtbObs.Document.Blocks.Clear();
            rtbObs.AppendText(obs);
            lblBattlePoints.Text = bp.ToString();
        }

        private void _admin_UpdatePlayersList(PlayerInfo[] players)
        {
            profiles = players;
            foreach (PlayerInfo player in players)
                if (!lbUserlist.Items.Contains(player.Username))
                    lbUserlist.Items.Add(player.Username);
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_ban, btn_kick, btn_mute, btn_save });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.Color1PanelButton;
                btn.Color2 = style.Color2PanelButton;
                btn.Update();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BCA_ColorButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tb_KickReason.GetText() != "" && profileselected != null)
                _admin.Client.Send(PacketType.Kick, new StandardClientKick { Reason = tb_KickReason.GetText(), Target = profileselected });
        }

        private void BCA_ColorButton_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (tb_MuteReason.GetText() != "" && profileselected != null && tb_MuteHours.Text != "")
                _admin.Client.Send(PacketType.Mute, new StandardClientMute { Reason = tb_MuteReason.GetText(), Target = profileselected, Time = Convert.ToInt32(tb_MuteHours.Text) });
        }

        private void BCA_ColorButton_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            if (tb_BanReason.GetText() != "" && profileselected != null && tb_BanHours.Text != "")
                _admin.Client.Send(PacketType.Mute, new StandardClientMute { Reason = tb_BanReason.GetText(), Target = profileselected, Time = Convert.ToInt32(tb_BanHours.Text) });
        }

        private void lbUserlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            profileselected = profiles[lbUserlist.SelectedIndex];
            _admin.Client.Send(PacketType.PanelAskProfile, new StandardClientAskPanelProfile { Player = profileselected });
        }

        private void btn_save_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            profileselected = profiles[lbUserlist.SelectedIndex];
            _admin.Client.Send(PacketType.PanelUpdate, new StandardClientPanelProfileUpdate { Player = profileselected, Observation = new TextRange(rtbObs.Document.ContentStart, rtbObs.Document.ContentEnd).Text});
        }
    }
}
