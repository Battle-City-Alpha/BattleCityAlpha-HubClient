using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Logique d'interaction pour PrestigeShop.xaml
    /// </summary>
    public partial class PrestigeShop : Window
    {
        InputText form = new InputText();
        public PrestigeShop()
        {
            InitializeComponent();

            InitItem();
        }

        private void InitItem()
        {
            VIP_Item.Initialize("Membre VIP", "500", "vip");
            Username_Item.Initialize("Changer de pseudo", "300", "change_username");
            Greet_Item.Initialize("Greet illimité", "200", "greet");
            DoubleBP_Item.Initialize("Doublez vos BPs (3J)", "500", "team");
            Avatar_Item.Initialize("Obtenir un avatar", "300", "avatar");
            PP_Item.Initialize("Obtenir des PPs", "", "pp");

            VIP_Item.btn_purchase.MouseLeftButtonDown += VIP_Item_MouseLeftButtonDown;
            Username_Item.btn_purchase.MouseLeftButtonDown += Username_Item_MouseLeftButtonDown;
            Greet_Item.btn_purchase.MouseLeftButtonDown += Greet_Item_MouseLeftButtonDown;
            DoubleBP_Item.btn_purchase.MouseLeftButtonDown += Team_Item_MouseLeftButtonDown;
            Avatar_Item.btn_purchase.MouseLeftButtonDown += Avatar_Item_MouseLeftButtonDown;
            PP_Item.btn_purchase.MouseLeftButtonDown += PP_Item_MouseLeftButtonDown;
        }

        private void PP_Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "http://battlecityalpha.xyz/Donation.html";
            p.Start();
        }

        private void Avatar_Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            form.SelectedText += Team_SelectedText; ;
            form.ShowDialog();
        }

        private void Team_SelectedText(string name)
        {
            FormExecution.Client.Send(PacketType.BuyTeam, new StandardClientBuyTeam
            {
                Name = name
            });
            form.SelectedText -= Team_SelectedText;
        }

        private void Team_Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Greet_Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.Client.Send(PacketType.BuyGreet, new StandardClientGreetIllimite());
        }

        private void Username_Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            form.SelectedText += Username_SelectedText;
            form.ShowDialog();
        }

        private void Username_SelectedText(string username)
        {
            FormExecution.Client.Send(PacketType.ChangeUsername, new StandardClientChangeUsername
            {
                Username = username
            });
            form.SelectedText -= Username_SelectedText;
        }

        private void VIP_Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.Client.Send(PacketType.BuyVIP, new StandardClientBuyVip());
        }
    }
}
