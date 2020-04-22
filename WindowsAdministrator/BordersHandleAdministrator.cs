using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using System;

namespace hub_client.WindowsAdministrator
{
    public class BordersHandleAdministrator
    {
        public GameClient Client;

        public event Action<Customization[]> LoadBorders;

        public BordersHandleAdministrator(GameClient client)
        {
            Client = client;
            Client.LoadBorders += Client_LoadBorders;
        }

        private void Client_LoadBorders(Customization[] borders)
        {
            LoadBorders?.Invoke(borders);
        }

        public void ChangeBorder(int id)
        {
            StandardClientChangeBorder packet = new StandardClientChangeBorder { Id = id };
            Client.Send(PacketType.ChangeBorder, packet);
        }
    }
}
