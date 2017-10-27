using BCA.Common;
using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class PanelAdministrator
    {
        public GameClient Client;

        public event Action<PlayerInfo[]> UpdatePlayersList;
        public event Action<string[], string, string, int> UpdateProfile;

        public PanelAdministrator(GameClient client)
        {
            Client = client;

            client.UpdatePanelUser += Client_UpdatePanelUser;
            client.UpdatePanelUserlist += Client_UpdatePanelUserlist;
        }

        private void Client_UpdatePanelUserlist(PlayerInfo[] players)
        {
            UpdatePlayersList?.Invoke(players);
        }

        private void Client_UpdatePanelUser(string[] accounts, string ip, string obs, int bp)
        {
            UpdateProfile?.Invoke(accounts, ip, obs, bp);
        }
    }
}
