using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace hub_client.WindowsAdministrator
{
    public class ChatAdministrator
    {
        public GameClient Client;

        public event Action LoginComplete;
        public event Action<Color, string, bool, bool> ChatMessage;
        public event Action<string> AddHubPlayer;
        public event Action<string> RemoveHubPlayer;

        public ChatAdministrator(GameClient client)
        {
            Client = client;
            Client.ChatMessageRecieved += Client_ChatMessageRecieved;
            Client.LoginComplete += Client_LoginComplete;
            Client.AddHubPlayer += Client_AddHubPlayer;
            Client.RemoveHubPlayer += Client_RemoveHubPlayer;
        }

        private void Client_RemoveHubPlayer(string username)
        {
            RemoveHubPlayer?.Invoke(username);
        }

        private void Client_AddHubPlayer(string username)
        {
            AddHubPlayer?.Invoke(username);
        }

        private void Client_LoginComplete()
        {
            LoginComplete?.Invoke();
        }

        private void Client_ChatMessageRecieved(Color color, string msg, bool italic, bool bold)
        {
            ChatMessage?.Invoke(color, msg, italic, bold);
        }
    }
}
