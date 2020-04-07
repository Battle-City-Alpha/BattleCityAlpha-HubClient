using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets;
using BCA.Network.Packets.Standard.FromClient;
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
    /// Logique d'interaction pour PopMessageBoxChoice.xaml
    /// </summary>
    public partial class ChoicePopBox : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        StandardClientDuelRequestAnswer packet;
        bool trade = false;

        public ChoicePopBox(PlayerInfo player, RoomConfig config, bool istrade)
        {
            InitializeComponent();

            string txt = String.Format("Vous avez été invité en duel par {0}. \r\n Type : {1}", player.Username, config.Type);
            if (istrade)
            {
                trade = true;
                txt = String.Format("Vous avez été invité en échange par {0}.", player.Username);
            }
            popText.Text = txt;

            Loaded += PopBox_Loaded;

            Title = "Requête de duel";
            packet = new StandardClientDuelRequestAnswer { Player = player, Config = config };
        }

        public void LoadStyle()
        {
            btnAgree.Color1 = style.GetGameColor("Color1PopBoxButton");
            btnAgree.Color2 = style.GetGameColor("Color2PopBoxButton");
            btnAgree.Update();

            btnAgainst.Color1 = style.GetGameColor("Color1PopBoxButton");
            btnAgainst.Color2 = style.GetGameColor("Color2PopBoxButton");
            btnAgainst.Update();

            this.FontFamily = style.Font;
        }

        private void PopBox_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
            Loaded -= PopBox_Loaded;
        }

        private void btnAgree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            packet.Result = true;
            SendAnswer();
            Close();
        }

        private void btnAgainst_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            packet.Result = false;
            SendAnswer();
            Close();
        }

        private void SendAnswer()
        {
            if (!trade)
                FormExecution.Client.Send(BCA.Network.Packets.Enums.PacketType.DuelRequestAnswer, packet);
            else
                FormExecution.Client.Send(BCA.Network.Packets.Enums.PacketType.TradeRequestAnswer, new StandardClientTradeRequestAnswer { Player = packet.Player, Result = packet.Result });
        }
    }
}
