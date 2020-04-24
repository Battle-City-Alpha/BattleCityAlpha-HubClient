using BCA.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace hub_client.Helpers
{
    public class BlacklistManager
    {
        public Dictionary<int, string> Blacklist;

        public BlacklistManager()
        {
            if (File.Exists("blacklist.json"))
                Blacklist = JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText("blacklist.json"));
            else
                Blacklist = new Dictionary<int, string>();
        }

        public void AddPlayer(PlayerInfo player)
        {
            if (!Blacklist.ContainsKey(player.UserId))
                Blacklist.Add(player.UserId, player.Username);
        }

        public void RemovePlayer(PlayerInfo player)
        {
            if (Blacklist.ContainsKey(player.UserId))
                Blacklist.Remove(player.UserId);
        }

        public bool CheckBlacklist(PlayerInfo player)
        {
            return Blacklist.ContainsKey(player.UserId);
        }

        public string GetList()
        {
            string bl = "Blacklist : ";
            foreach (var player in Blacklist)
                bl += player.Value + ",";
            return bl.Substring(bl.Length - 2);
        }

        public void Save()
        {
            File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(Blacklist));
        }
    }
}
