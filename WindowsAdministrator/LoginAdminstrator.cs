using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class LoginAdminstrator
    {
        public GameClient Client;

        public event Action LoginComplete;

        public LoginAdminstrator(GameClient client)
        {
            Client = client;
            Client.LoginComplete += LoginAdminstrator_LoginComplete;
        }

        private void LoginAdminstrator_LoginComplete()
        {
            LoginComplete?.Invoke();
        }
    }
}
