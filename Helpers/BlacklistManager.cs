using BCA.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace hub_client.Helpers
{
    public class BlacklistManager
    {
        public List<PlayerInfo> Blacklist;

        public BlacklistManager()
        {
            if (File.Exists("blacklist.json"))
                Blacklist = JsonConvert.DeserializeObject<List<PlayerInfo>>(File.ReadAllText("blacklist.json"));
            else
                Blacklist = new List<PlayerInfo>();
        }

        public void AddPlayer(PlayerInfo player)
        {
            if (!Blacklist.Contains(player))
                Blacklist.Add(player);
        }

        public void RemovePlayer(PlayerInfo player)
        {
            if (Blacklist.Contains(player))
                Blacklist.Remove(player);
        }

        public bool CheckBlacklist(PlayerInfo player)
        {
            return Blacklist.Contains(player);
        }

        public string GetList()
        {
            string bl = "Blacklist : ";
            foreach (PlayerInfo info in Blacklist)
                bl += info.Username + ",";
            return bl.Substring(bl.Length - 2);
        }

        public void Save()
        {
            File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(Blacklist));
        }
    }
}
