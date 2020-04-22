using BCA.Network.Packets;
using BCA.Network.Packets.Enums;

namespace hub_client.Network
{
    public class NetworkData
    {
        public PacketType Type { get; set; }
        public Packet Packet { get; set; }

        public NetworkData(PacketType type, Packet packet)
        {
            Type = type;
            Packet = packet;
        }
    }
}
