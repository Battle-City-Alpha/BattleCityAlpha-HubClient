using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class PrivateMessageAdministrator
    {
        public GameClient Client;

        public event Action<string> MessageRecieved;

        public PrivateMessageAdministrator(GameClient client)
        {
            Client = client;
        }

        public void PrivateMessageRecieved(string msg)
        {
            MessageRecieved?.Invoke(msg);
        }
    }
}
