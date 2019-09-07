using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class BrocanteAdministrator
    {
        public GameClient Client;

        public event Action<List<BrocanteCard>> LoadBrocante;

        public BrocanteAdministrator(GameClient client)
        {
            Client = client;

            Client.LoadBrocante += Client_LoadBrocante;
        }

        private void Client_LoadBrocante(List<BrocanteCard> cards)
        {
            LoadBrocante?.Invoke(cards);
        }

        public void AskBrocante()
        {
            Client.Send(PacketType.LoadBrocante, new StandardClientAskBrocante());
        }
        public void CloseBrocante()
        {
            Client.Send(PacketType.CloseBrocante, new StandardClientCloseBrocante());
        }
        public void AskSelectCard()
        {
            Client.Send(PacketType.AskSelectCard, new StandardClientAskSelectCard());
        }
        public void SellBrocanteCard(PlayerCard card, int price, int quantity)
        {
            Client.Send(PacketType.SellBrocanteCard, new StandardClientSellBrocanteCard
            {
                CardId = card.Id,
                Price = price,
                Quantity = quantity
            });
        }
        public void BuyBrocanteCard(BrocanteCard card)
        {
            if (card == null)
                return;
            Client.Send(PacketType.BuyBrocanteCard, new StandardClientBuyBrocanteCard
            {
                BcId = card.BCId
            });
        }
    }
}
