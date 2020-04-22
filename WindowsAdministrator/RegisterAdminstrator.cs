using hub_client.Network;
using System;

namespace hub_client.WindowsAdministrator
{
    public class RegisterAdminstrator
    {
        public GameClient Client;

        public event Action RegistrationComplete;

        public RegisterAdminstrator(GameClient client)
        {
            Client = client;
            client.RegistrationComplete += Client_RegistrationComplete;
        }

        private void Client_RegistrationComplete()
        {
            RegistrationComplete?.Invoke();
        }
    }
}
