using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class ChatAdministrator
    {
        public GameClient Client;

        public event Action<string> ChatMessage;

        public ChatAdministrator(GameClient client)
        {
            Client = client;
            Client.ChatMessageRecieved += Client_ChatMessageRecieved;
        }

        private void Client_ChatMessageRecieved(string message)
        {
            ChatMessage?.Invoke(message);
        }
    }
}
