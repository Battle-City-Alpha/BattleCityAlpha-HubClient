using BCA.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace hub_client.Helpers
{
    public class PlayerManager
    {
        public Dictionary<int, PlayerInfo> Players;

        public Dictionary<int, PlayerCard> Collections;

        public PlayerManager()
        {
            Players = new Dictionary<int, PlayerInfo>();
            Collections = new Dictionary<int, PlayerCard>();
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
                if (info.Value.Username.ToUpper() == username.ToUpper())
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
            Collections = cards;
        }

        public Dictionary<int, PlayerCard> LoadCoffre()
        {
            Collections = new Dictionary<int, PlayerCard>();
            foreach (string line in File.ReadLines(Path.Combine(FormExecution.path, "BattleCityAlpha", "chest.list")))
                Collections.Add(Convert.ToInt32(line.Split(' ')[0]), new PlayerCard {
                    Quantity = Convert.ToInt32(line.Split(' ')[1]),
                    Id = Convert.ToInt32(line.Split(' ')[0])
                });
            return Collections;
        }
        public void AddCard(int id, int qty)
        {
            if (Collections.ContainsKey(id))
                Collections[id].Quantity += qty;
            else
                Collections.Add(id, new PlayerCard
                {
                    Id = id,
                    Quantity = qty
                });
        }
    }
}
