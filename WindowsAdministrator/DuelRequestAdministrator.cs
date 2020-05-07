using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using NLog;

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

        public void SendRequest(int id, string password, RoomType roomtype, int banlist, RoomRules rules, int cardsbyhand, int startduellp, int masterrules, int drawcount, bool noShuffleDeck, string captiontext)
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
                    StartDuelLP = startduellp,
                    MasterRules = masterrules,
                    DrawCount = drawcount,
                    NoShuffleDeck = noShuffleDeck,
                    CaptionText = captiontext
                },
                RoomPass = password
            });
        }
        public void SendHost(RoomType roomtype, string password, int banlist, RoomRules rules, int cardsbyhand, int startduellp, int masterrules, int drawcount, bool noShuffleDeck, string captiontext)
        {
            Client.Send(PacketType.DuelHost, new StandardClientDuelHost
            {
                Config = new RoomConfig
                {
                    Type = roomtype,
                    Banlist = banlist,
                    Rules = rules,
                    CardByHand = cardsbyhand,
                    StartDuelLP = startduellp,
                    MasterRules = masterrules,
                    DrawCount = drawcount,
                    NoShuffleDeck = noShuffleDeck,
                    CaptionText = captiontext
                },
                RoomPass = password
            });
        }
    }
}
