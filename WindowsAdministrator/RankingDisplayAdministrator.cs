using BCA.Common;
using hub_client.Network;
using System;

namespace hub_client.WindowsAdministrator
{
    public class RankingDisplayAdministrator
    {
        public GameClient Client;

        public event Action<RankingPlayerInfos[], Customization[]> ShowRanking;

        public RankingDisplayAdministrator(GameClient client)
        {
            Client = client;
            Client.ShowRanking += Client_ShowRanking;
        }

        private void Client_ShowRanking(RankingPlayerInfos[] infos, Customization[] customs)
        {
            ShowRanking?.Invoke(infos, customs);
        }
    }
}
