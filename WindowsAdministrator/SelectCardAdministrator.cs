using BCA.Common;
using hub_client.Network;
using System;
using System.Collections.Generic;

namespace hub_client.WindowsAdministrator
{
    public class SelectCardAdministrator
    {
        public GameClient Client;

        public event Action<Dictionary<int, PlayerCard>, bool> LoadSelectCard;

        public SelectCardAdministrator(GameClient client)
        {
            Client = client;

            Client.LoadSelectCard += Client_LoadSelectCard;
        }

        private void Client_LoadSelectCard(Dictionary<int, PlayerCard> cards, bool cardSold)
        {
            LoadSelectCard?.Invoke(cards, cardSold);
        }
    }
}
