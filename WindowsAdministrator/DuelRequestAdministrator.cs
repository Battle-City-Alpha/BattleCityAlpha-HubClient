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
    public class DuelRequestAdministrator
    {
        public GameClient Client;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public DuelRequestAdministrator(GameClient client)
        {
            Client = client;
        }

        public void SendRequest(int id, RoomType roomtype, int banlist, RoomRules rules, int cardsbyhand, int startduellp)
        {
            Client.Send(PacketType.DuelRequest, new StandardClientDuelRequest
            {
                TargetId = id,
                Config = new RoomConfig
                {
                    Type = roomtype,
                    Banlist = banlist,
                    Rules = rules,
                    CardByHand = cardsbyhand,
                    StartDuelLP = startduellp
                }
            });
        }
        public void SendHost(RoomType roomtype, int banlist, RoomRules rules, int cardsbyhand, int startduellp)
        {
            Client.Send(PacketType.DuelHost, new StandardClientDuelHost
            {
                Config = new RoomConfig
                {
                    Type = roomtype,
                    Banlist = banlist,
                    Rules = rules,
                    CardByHand = cardsbyhand,
                    StartDuelLP = startduellp
                }
            });
        }
    }
}
