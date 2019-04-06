using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Cards;
using hub_client.Stuff;
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
    /// Logique d'interaction pour Brocante.xaml
    /// </summary>
    public partial class Brocante : Window
    {
        BrocanteAdministrator _admin;

        public Brocante(BrocanteAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;

            _admin.LoadBrocante += _admin_LoadBrocante;
        }

        private void _admin_LoadBrocante(List<BrocanteCard> cards)
        {
            brocanteList.ItemsSource = null;

            foreach (BrocanteCard card in cards)
                card.CardName = CardManager.GetCard(card.Id).Name;

            brocanteList.ItemsSource = cards;

            _admin.Client.OpenPopBox("Les cartes dans la brocante ont été mises à jour.", "Mise à jour brocante");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _admin.Client.Send(PacketType.LoadBrocante, new StandardClientAskBrocante());
        }

        private void brocanteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BrocanteCard card = ((sender as ListView).SelectedItem as BrocanteCard);

            if (card == null)
                return;

            DisplayCardInfo.SetCard(CardManager.GetCard(card.Id));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _admin.Client.Send(PacketType.CloseBrocante, new StandardClientCloseBrocante());
            _admin.LoadBrocante -= _admin_LoadBrocante;
        }

        private void btnSell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.Client.Send(PacketType.AskSelectCard, new StandardClientAskSelectCard());
            SelectCard form = new SelectCard(_admin.Client.SelectCardAdmin);
            form.SelectedCard += Form_SelectedCard;
            form.ShowDialog();
            form.SelectedCard -= Form_SelectedCard;
        }

        private void Form_SelectedCard(PlayerCard card, int price, int quantity)
        {
            _admin.Client.Send(PacketType.SellBrocanteCard, new StandardClientSellBrocanteCard
            {
                CardId = card.Id,
                Price = price,
                Quantity = quantity
            });
        }

        private void btnBuy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BrocanteCard card = (brocanteList.SelectedItem as BrocanteCard);
            _admin.Client.Send(PacketType.BuyBrocanteCard, new StandardClientBuyBrocanteCard
            {
                BcId = card.BCId
            });
        }
    }
}
