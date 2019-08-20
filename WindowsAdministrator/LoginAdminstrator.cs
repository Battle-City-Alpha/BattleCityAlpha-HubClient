using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
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

        public void SendAuthentification(string username, string password, string encryptKey, string HID)
        {
            Client.Send(PacketType.Login, new StandardClientLogin
            {
                Username = username,
                Password = CryptoManager.Encryption(password, encryptKey),
                HID = HID
            });

            Client.InitPlayer(username);
        }
    }
}
