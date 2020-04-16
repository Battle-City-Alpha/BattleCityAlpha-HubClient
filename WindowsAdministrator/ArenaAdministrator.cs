using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
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
    public class ArenaAdministrator
    {
        public GameClient Client;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Dictionary<int, Room> Rooms;

        public event Action<Room, bool> UpdateRoom;
        
        public ArenaAdministrator(GameClient client)
        {
            Client = client;
            Client.UpdateRoom += Client_UpdateRoom;
            Rooms = new Dictionary<int, Room>();
        }

        private void Client_UpdateRoom(Room obj, bool remove)
        {
            UpdateRoom?.Invoke(obj, remove);

            if (!remove)
                Rooms[obj.Id] = obj;
            else
            {
                if (Rooms.ContainsKey(obj.Id))
                    Rooms.Remove(obj.Id);
            }
                    
        }

        public void SendJoinRoom(int id, RoomType type, string pass)
        {
            Client.Send(PacketType.DuelJoin, new StandardClientDuelJoin
            {
                Id = id,
                RoomType = type,
                RoomPass = pass
            });

            logger.Trace("Try join duel - " + id.ToString());
        }

        /*public void SendDuelSeek(RoomType type)
        {
            Client.Send(PacketType.DuelSeeker, new StandardClientDuelSeeker
            {
                Type = type
            });
            logger.Trace("Duel Seeker - " + type.ToString());
        }*/
    }
}
