using BCA.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Helpers
{
    public class PlayerManager
    {
        public Dictionary<string, PlayerInfo> Players;

        public PlayerManager()
        {
            Players = new Dictionary<string, PlayerInfo>();
        }

        public void UpdatePlayer(PlayerInfo infos)
        {
            if (Players.ContainsKey(infos.Username))
                Players[infos.Username] = infos;
            else
                Players.Add(infos.Username, infos);
        }


        public void Remove(PlayerInfo infos)
        {
            if (Players.ContainsKey(infos.Username))
                Players.Remove(infos.Username);
        }

        public PlayerInfo GetInfos(string username)
        {
            if (Players.ContainsKey(username))
                return Players[username];
            return null;
        }

        public void UpdateCollection(Dictionary<int, PlayerCard> cards)
        {
            string file = "";
            foreach (var card in cards)
                file += card.Key + ":" + card.Value.Quantity + Environment.NewLine;
            File.Delete(Path.Combine(FormExecution.path, "BattleCityAlpha", "chest.list"));
            File.WriteAllText(Path.Combine(FormExecution.path, "BattleCityAlpha", "chest.list"), file);
        }
    }
}
