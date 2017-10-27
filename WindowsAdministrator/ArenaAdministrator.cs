using BCA.Common;
using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class ArenaAdministrator
    {
        public GameClient Client;

        public event Action<Room> UpdateRoom;
        
        public ArenaAdministrator(GameClient client)
        {
            Client = client;
            Client.UpdateRoom += Client_UpdateRoom;
        }

        private void Client_UpdateRoom(Room obj)
        {
            UpdateRoom?.Invoke(obj);
        }
    }
}
