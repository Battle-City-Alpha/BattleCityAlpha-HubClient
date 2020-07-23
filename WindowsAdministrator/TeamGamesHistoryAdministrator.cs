using BCA.Common;
using hub_client.Network;
using hub_client.Windows;
using System;

namespace hub_client.WindowsAdministrator
{
    public class TeamGamesHistoryAdministrator
    {
        public GameClient Client;

        public event Action<TeamGameResult[]> ShowResults;

        public TeamGamesHistoryAdministrator(GameClient client)
        {
            Client = client;

            Client.TeamGamesHistory += Client_TeamGamesHistory;
        }

        private void Client_TeamGamesHistory(TeamGameResult[] results)
        {
            TeamGamesHistory window = new TeamGamesHistory(this);
            ShowResults?.Invoke(results);
        }
    }
}
