using BCA.Common;
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
    }
}
