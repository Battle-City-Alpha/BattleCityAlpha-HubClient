using hub_client.Network;
using System;

namespace hub_client.WindowsAdministrator
{
    public class PurchaseAdministrator
    {
        public GameClient Client;
        public event Action<int[]> PurchaseItem;

        public PurchaseAdministrator(GameClient client)
        {
            Client = client;
            client.PurchaseItem += Client_PurchaseItem; ;
        }

        private void Client_PurchaseItem(int[] cards)
        {
            PurchaseItem?.Invoke(cards);
        }
    }
}
