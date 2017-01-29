using BCA.Network;
using BCA.Network.Packets;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromServer;
using hub_client.WindowsAdministrator;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace hub_client.Network
{
    public class GameClient : BinaryClient
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private string _username;
        private string _password;

        #region ChatForm Events
        public event Action<string> ChatMessageRecieved;
        #endregion

        #region Administrator
        public ChatAdministrator ChatAdmin;
        #endregion

        public GameClient() : base(new NetworkClient())
        {
            PacketReceived += GameClient_PacketReceived;
            Disconnected += Client_Disconnected;
            InitAdministrator();
        }

        private void InitAdministrator()
        {
            ChatAdmin = new ChatAdministrator(this);
        }

        public string Username()
        {
            return _username;
        }

        public void InitPlayer(string username, string password)
        {
            _username = username;
            _password = password;
        }

        private void Client_Disconnected(Exception ex)
        {
            logger.Fatal(ex);
            //TODO Display Message.
        }

        public void Send(PacketType packetId, Packet packet)
        {
            logger.Trace("PACKET SEND - {0} : {1}", packetId, packet);
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write((short)packetId);
                    writer.Write(JsonConvert.SerializeObject(packet));
                }
                Send(stream.ToArray());
            }
        }

        private void GameClient_PacketReceived(BinaryReader reader)
        {
            PacketType packetType = (PacketType)reader.ReadInt16();
            string packet = reader.ReadString();

            logger.Trace("PACKET RECEIVED - {0} : {1}", packetType, packet);

            switch (packetType)
            {
                case PacketType.ChatMessage:
                    HandleChatMessage(packet);
                    break;
            }
        }

        private void HandleChatMessage(string packetRecieved)
        {
            StandardServerChatMessage packet = JsonConvert.DeserializeObject<StandardServerChatMessage>(packetRecieved);
            string sRecieved = "[" + packet.Username + "]:" + packet.Message;
            Application.Current.Dispatcher.InvokeAsync(() => ChatMessageRecieved?.Invoke(sRecieved));
            logger.Info(sRecieved);
            logger.Trace("CHAT MESSAGE - {0}", sRecieved);
        }

    }
}
