using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class ToolsAdministrator
    {
        public GameClient Client;

        public event Action<int[]> LoadAvatars;

        public ToolsAdministrator(GameClient client)
        {
            Client = client;

            Client.LoadAvatars += Client_LoadAvatars;
        }

        private void Client_LoadAvatars(int[] avatars)
        {
            LoadAvatars?.Invoke(avatars);
        }
    }
}
