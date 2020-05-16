using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using hub_client.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class MonthPackViewerAdministrator
    {
        public GameClient Client;

        public MonthPackViewerAdministrator(GameClient client)
        {
            Client = client;

            Client.LoadMonthPack += Client_LoadMonthPack;
        }

        private void Client_LoadMonthPack(int avatar, int border, int sleeve)
        {
            MonthPackViewer viewer = new MonthPackViewer(this, avatar, border, sleeve);
            viewer.Show();
        }

        public void SendBuyMonthPack()
        {
            Client.Send(PacketType.BuyMonthPack, new StandardClientBuyMonthPack { });
        }
    }
}
