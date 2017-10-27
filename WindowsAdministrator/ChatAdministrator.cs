using BCA.Common;
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
        public event Action<PlayerInfo> AddHubPlayer;
        public event Action<PlayerInfo> RemoveHubPlayer;
        public event Action<string, string> ClearChat;

        public ChatAdministrator(GameClient client)
        {
            Client = client;
            Client.ChatMessageRecieved += Client_ChatMessageRecieved;
            Client.LoginComplete += Client_LoginComplete;
            Client.AddHubPlayer += Client_AddHubPlayer;
            Client.RemoveHubPlayer += Client_RemoveHubPlayer;
            Client.ClearChat += Client_ClearChat;
            Client.Banlist += Client_Banlist;
        }

        private void Client_Banlist(string[] players)
        {
            string bl = "Banlist : ";
            foreach (string player in players)
                bl += player + ",";

            ChatMessage?.Invoke(FormExecution.AppDesignConfig.StaffMessageColor, bl, false, false);
        }

        private void Client_ClearChat(string username, string reason)
        {
            ClearChat?.Invoke(username, reason);
        }

        private void Client_RemoveHubPlayer(PlayerInfo infos)
        {
            RemoveHubPlayer?.Invoke(infos);
        }

        private void Client_AddHubPlayer(PlayerInfo infos)
        {
            AddHubPlayer?.Invoke(infos);
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
