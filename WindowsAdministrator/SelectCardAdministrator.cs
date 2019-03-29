using BCA.Common;
using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class SelectCardAdministrator
    {
        public GameClient Client;

        public event Action<Dictionary<int, PlayerCard>> LoadSelectCard;

        public SelectCardAdministrator(GameClient client)
        {
            Client = client;

            Client.LoadSelectCard += Client_LoadSelectCard;
        }

        private void Client_LoadSelectCard(Dictionary<int, PlayerCard> cards)
        {
            LoadSelectCard?.Invoke(cards);
        }
    }
}
