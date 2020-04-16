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
        public Dictionary<int, Room> WaitingRooms;
        public Dictionary<int, Room> DuelingRooms;

        public event Action<Room> UpdateRoom;
        
        public ArenaAdministrator(GameClient client)
        {
            Client = client;
            Client.UpdateRoom += Client_UpdateRoom;

            WaitingRooms = new Dictionary<int, Room>();
            DuelingRooms = new Dictionary<int, Room>();
        }

        private void Client_UpdateRoom(Room obj)
        {
            UpdateRoom?.Invoke(obj);

            switch(obj.State)
            {
                case RoomState.Waiting:
                    WaitingRooms[obj.Id] = obj;
                    break;
                case RoomState.Dueling:
                    if (WaitingRooms.ContainsKey(obj.Id))
                        WaitingRooms.Remove(obj.Id);
                    DuelingRooms.Add(obj.Id, obj);
                    break;
                case RoomState.Finished:
                    if (WaitingRooms.ContainsKey(obj.Id))
                        WaitingRooms.Remove(obj.Id);
                    if (DuelingRooms.ContainsKey(obj.Id))
                        DuelingRooms.Remove(obj.Id);
                    break;
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

        public void SendPlayRanked()
        {
            Client.Send(PacketType.PlayRanked, new StandardClientPlayRanked { });
            logger.Trace("Send Play Ranked");
        }
        public void SendStopPlayRanked()
        {
            Client.Send(PacketType.StopPlayRanked, new StandardClientStopPlayRanked { });
            logger.Trace("Send Stop Play Ranked");
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
