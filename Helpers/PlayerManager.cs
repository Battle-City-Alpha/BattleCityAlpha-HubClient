using BCA.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace hub_client.Helpers
{
    public class PlayerManager
    {
        public Dictionary<int, PlayerInfo> Players;

        public PlayerManager()
        {
            Players = new Dictionary<int, PlayerInfo>();
        }

        public void UpdatePlayer(PlayerInfo infos)
        {
            if (Players.ContainsKey(infos.UserId))
                Players[infos.UserId] = infos;
            else
                Players.Add(infos.UserId, infos);
        }


        public void Remove(PlayerInfo infos)
        {
            if (Players.ContainsKey(infos.UserId))
                Players.Remove(infos.UserId);
        }

        public PlayerInfo GetInfos(string username)
        {
            foreach (var info in Players)
                if (info.Value.Username == username)
                    return info.Value;
            return null;
        }

        public void UpdateCollection(Dictionary<int, PlayerCard> cards)
        {
            string file = "";
            foreach (var card in cards)
                file += card.Key + " " + card.Value.Quantity + Environment.NewLine;
            File.Delete(Path.Combine(FormExecution.path, "BattleCityAlpha", "Chest.list"));
            File.WriteAllText(Path.Combine(FormExecution.path, "BattleCityAlpha", "Chest.list"), file);
        }
    }
}
