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
    public class TradeAdministrator
    {
        public GameClient Client;

        public event Action<int, PlayerInfo[], Dictionary<int, PlayerCard>[]> InitTrade;
        public event Action<PlayerInfo, string> GetMessage;
        public event Action<List<PlayerCard>> UpdateCardsToOffer;
        public event Action TradeExit;
        public event Action TradeEnd;

        public TradeAdministrator(GameClient client)
        {
            Client = client;

            Client.InitTrade += Client_InitTrade;
            Client.GetMessage += Client_GetMessage;
            Client.UpdateCardsToOffer += Client_UpdateCardsToOffer;
            Client.TradeExit += Client_TradeExit;
            Client.TradeEnd += Client_TradeEnd;
        }

        private void Client_TradeEnd()
        {
            TradeEnd?.Invoke();
        }

        private void Client_TradeExit()
        {
            TradeExit?.Invoke();
        }

        private void Client_UpdateCardsToOffer(List<PlayerCard> cards)
        {
            UpdateCardsToOffer?.Invoke(cards);
        }

        private void Client_GetMessage(PlayerInfo infos, string message)
        {
            GetMessage?.Invoke(infos, message);
        }

        private void Client_InitTrade(int arg1, PlayerInfo[] arg2, Dictionary<int, PlayerCard>[] arg3)
        {
            InitTrade?.Invoke(arg1, arg2, arg3);
        }

        public void SendTradeAnswer(int id, bool result)
        {
            Client.Send(PacketType.TradeAnswer, new StandardClientTradeAnswer { Id = id, Result = result });
        }
        public void SendTradeProposition(int id, List<PlayerCard> cards)
        {
            Client.Send(PacketType.TradeProposition, new StandardClientTradeProposition { Id = id, Cards = cards });
        }
        public void SendTradeExit(int id)
        {
            Client.Send(PacketType.TradeExit, new StandardClientTradeExit { Id = id });
        }
        public void SendTradeMessage(int id, string msg)
        {
            Client.Send(PacketType.TradeMessage, new StandardClientTradeMessage { Id = id, Message = msg });
        }
    }
}
