using BCA.Common;
using BCA.Common.Enums;
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

        public ChoicePopBox(PlayerInfo player, DuelType type)
        {
            string txt = String.Format("Vous avez été invité en duel par {0}. \r\n Type : {1}", player.Username, type);
            InitializeComponent();
            popText.Text = txt;
            Loaded += PopBox_Loaded;
            Title = "Requête de duel";

            packet = new StandardClientDuelRequestAnswer { Player = player, Type = type };
        }

        public void LoadStyle()
        {
            btnAgree.Color1 = style.Color1HomeHeadButton;
            btnAgree.Color2 = style.Color2HomeHeadButton;
            btnAgree.Update();
        }

        private void PopBox_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
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
            FormExecution.Client.Send(BCA.Network.Packets.Enums.PacketType.DuelRequestAnswer, packet);
        }
    }
}
