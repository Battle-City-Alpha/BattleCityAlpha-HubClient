using BCA.Common;
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
        public event Action<string, string> GetMessage;
        public event Action<List<PlayerCard>> UpdateCardsToOffer;
        public event Action TradeExit;

        public TradeAdministrator(GameClient client)
        {
            Client = client;

            Client.InitTrade += Client_InitTrade;
            Client.GetMessage += Client_GetMessage;
            Client.UpdateCardsToOffer += Client_UpdateCardsToOffer;
            Client.TradeExit += Client_TradeExit;
            
        }

        private void Client_TradeExit()
        {
            TradeExit?.Invoke();
        }

        private void Client_UpdateCardsToOffer(List<PlayerCard> cards)
        {
            UpdateCardsToOffer?.Invoke(cards);
        }

        private void Client_GetMessage(string username, string message)
        {
            GetMessage?.Invoke(username, message);
        }

        private void Client_InitTrade(int arg1, PlayerInfo[] arg2, Dictionary<int, PlayerCard>[] arg3)
        {
            InitTrade?.Invoke(arg1, arg2, arg3);
        }
    }
}
