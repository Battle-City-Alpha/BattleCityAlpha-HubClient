using BCA.Common;
using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class TradeAdministrator
    {
        public GameClient Client;

        public event Action<int, PlayerInfo[], Dictionary<int, PlayerCard>[]> InitTrade;

        public TradeAdministrator(GameClient client)
        {
            Client = client;

            Client.InitTrade += Client_InitTrade; ;
        }

        private void Client_InitTrade(int arg1, PlayerInfo[] arg2, Dictionary<int, PlayerCard>[] arg3)
        {
            InitTrade?.Invoke(arg1, arg2, arg3);
        }
    }
}
