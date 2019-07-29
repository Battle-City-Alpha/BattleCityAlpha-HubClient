using BCA.Common;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class PrivateMessageAdministrator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
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

        public void SendMessage(PlayerInfo target, string msg)
        {
            NetworkData data = new NetworkData(BCA.Network.Packets.Enums.PacketType.PrivateMessage, new StandardClientPrivateMessage { Target = target, Message = msg});
            Client.Send(data);
            logger.Info("Private message sent : {0}.", data);
        }
    }
}
