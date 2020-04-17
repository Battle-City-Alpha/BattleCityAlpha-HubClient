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
    public class SleevesHandleAdministrator
    {
        public GameClient Client;

        public event Action<Customization[]> LoadSleeves;
        public SleevesHandleAdministrator(GameClient client)
        {
            Client = client;
            Client.LoadSleeves += Client_LoadSleeves;
        }

        private void Client_LoadSleeves(Customization[] sleeves)
        {
            LoadSleeves?.Invoke(sleeves);
        }

        public void ChangeSleeve(int id)
        {
            StandardClientChangeSleeve packet = new StandardClientChangeSleeve { Id = id };
            Client.Send(PacketType.ChangeSleeve, packet);
        }
    }
}
