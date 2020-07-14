using BCA.Common;
using BCA.Network.Packets.Standard.FromServer;
using hub_client.Network;
using hub_client.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class TeamProfileAdministrator
    {
        public GameClient Client;
        public event Action<int, string, string, string, int, int, int, int, int, PlayerInfo[], int> LoadTeamProfile;


        public TeamProfileAdministrator(GameClient client)
        {
            Client = client;
            Client.TeamProfileInfos += Client_TeamProfileInfos;
        }

        private void Client_TeamProfileInfos(StandardServerAskTeam infos)
        {
            if (!FormExecution.IsWindowOpen<TeamProfile>())
            {
                TeamProfile tp = new TeamProfile(this);
            }
            LoadTeamProfile?.Invoke(infos.ID, infos.Emblem, infos.Name, infos.Tag, infos.Wins, infos.Loses, infos.Rank, infos.LeaderID, infos.ColeaderID, infos.Members, infos.Score);
        }
    }
}
