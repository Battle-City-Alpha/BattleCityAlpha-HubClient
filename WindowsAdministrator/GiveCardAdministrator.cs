using BCA.Common;
using hub_client.Network;
using System;
using System.Collections.Generic;

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
    }
}
