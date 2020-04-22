using BCA.Common;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Configuration;
using System;
using System.Windows;
using System.Windows.Input;

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
        string pass = "";

        public ChoicePopBox(PlayerInfo player, RoomConfig config, bool istrade, string pass)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            string txt = String.Format("Vous avez été invité en duel par {0}. \r\n Type : {1}", player.Username, config.Type);
            if (pass != string.Empty)
                txt += Environment.NewLine + "Partie privée";
            Title = "Requête de duel";
            if (istrade)
            {
                Title = "Requête d'échange";
                trade = true;
                txt = String.Format("Vous avez été invité en échange par {0}.", player.Username);
            }
            popText.Text = txt;

            Loaded += PopBox_Loaded;

            packet = new StandardClientDuelRequestAnswer { Player = player, Config = config, Roompass = pass };
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
