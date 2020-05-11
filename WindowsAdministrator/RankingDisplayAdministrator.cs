using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
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

        public void SendAskProfil(int userID)
        {
            Client.Send(PacketType.Profil, new StandardClientProfilAsk
            {
                UserID = userID
            });
        }
    }
}
