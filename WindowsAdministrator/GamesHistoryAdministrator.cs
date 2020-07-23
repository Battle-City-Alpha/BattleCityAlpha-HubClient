using BCA.Common;
using hub_client.Network;
using hub_client.Windows.Controls.Controls_Stuff;
using NLog;
using System;
using System.Collections.Generic;

namespace hub_client.WindowsAdministrator
{
    public class GamesHistoryAdministrator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public GameClient Client;

        public event Action<RoomResultItem[]> GetGamesHistory;

        public GamesHistoryAdministrator(GameClient client)
        {
            Client = client;

            Client.GetGamesHistory += Client_GetGamesHistory;
        }

        private void Client_GetGamesHistory(RoomResult[] results)
        {
            List<RoomResultItem> items = new List<RoomResultItem>();

            foreach (RoomResult r in results)
            {
                RoomResultItem item = new RoomResultItem
                {
                    GameDate = r.GameDate.ToString("dd/MM"),
                    CurrentRank = r.CurrentRank,
                    ELO = r.ELO,
                    Opponent = r.Opponent.Username,
                    TotalRank = r.TotalRank,
                    ResultPic = r.ELO > 0 ? FormExecution.AssetsManager.GetImage("Logo", "win") : FormExecution.AssetsManager.GetImage("Logo", "lose")
                };

                item.AvatarPic = FormExecution.AssetsManager.GetCustom(r.Opponent.Avatar);
                items.Add(item);
            }

            GetGamesHistory?.Invoke(items.ToArray());
        }

    }
}
