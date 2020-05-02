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
    public class GiveCardAdministrator
    {
        public GameClient Client;

        public event Action<Dictionary<int, PlayerCard>> LoadCards;

        public GiveCardAdministrator(GameClient client)
        {
            Client = client;

            Client.LoadGiveCards += Client_LoadGiveCards; ;
        }

        private void Client_LoadGiveCards(Dictionary<int, PlayerCard> cards)
        {
            LoadCards?.Invoke(cards);
        }
        public void SendGiveCard(Dictionary<int, PlayerCard> cards, PlayerInfo target)
        {
            Client.Send(PacketType.CardDonation, new StandardClientCardDonation
            {
                Target = target,
                Cards = cards
            });
        }
    }
}
