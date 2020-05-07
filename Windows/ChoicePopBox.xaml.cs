using BCA.Common;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Configuration;
using hub_client.Enums;
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
        ChoiceBoxType _type = ChoiceBoxType.Duel;
        string pass = "";

        public event Action<bool> Choice;

        public ChoicePopBox(PlayerInfo player, RoomConfig config, ChoiceBoxType type, string pass = "", string deckname = "")
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            string txt = string.Empty;

            _type = type;
            switch (_type)
            {
                case ChoiceBoxType.Duel:
                    txt = String.Format("Vous avez été invité en duel par {0}. \r\nType : {1}", player.Username, config.Type);
                    txt += Environment.NewLine + string.Format("MasterRules : {0}", config.MasterRules);
                    txt += Environment.NewLine + string.Format("Banlist : {0}", FormExecution.GetBanlistValue(config.Banlist));
                    txt += Environment.NewLine + string.Format("Point de vie : {0}", config.StartDuelLP);
                    txt += Environment.NewLine + string.Format("Carte dans la main au départ : {0}", config.CardByHand);
                    txt += Environment.NewLine + string.Format("Pioche par tour : {0}", config.DrawCount);
                    txt += Environment.NewLine + string.Format("Info : {0}", config.CaptionText);
                    txt += Environment.NewLine + (config.NoShuffleDeck ? "Deck non mélangé" : "Deck mélangé");
                    if (pass != string.Empty)
                        txt += Environment.NewLine + "Partie privée";
                    Title = "Requête de duel";
                    break;
                case ChoiceBoxType.Trade:
                    Title = "Requête d'échange";
                    txt = String.Format("Vous avez été invité en échange par {0}.", player.Username);
                    break;
                case ChoiceBoxType.Deck:
                    txt = "Vous avez reçu le deck " + deckname + " de la part de " + player.Username + ".";
                    break;
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
            if (_type != ChoiceBoxType.Deck)
            {
                packet.Result = true;
                SendAnswer();
            }
            else
            {
                Choice?.Invoke(true);
            }
            Close();
        }

        private void btnAgainst_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_type != ChoiceBoxType.Deck)
            {
                packet.Result = false;
                SendAnswer();
            }
            Close();
        }

        private void SendAnswer()
        {
            if (_type != ChoiceBoxType.Trade)
                FormExecution.Client.Send(BCA.Network.Packets.Enums.PacketType.DuelRequestAnswer, packet);
            else
                FormExecution.Client.Send(BCA.Network.Packets.Enums.PacketType.TradeRequestAnswer, new StandardClientTradeRequestAnswer { Player = packet.Player, Result = packet.Result });
        }
    }
}
