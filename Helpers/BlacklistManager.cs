using BCA.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Blacklist.Add(player);
        }

        public void RemovePlayer(PlayerInfo player)
        {
            Blacklist.Remove(player);
        }

        public bool CheckBlacklist(PlayerInfo player)
        {
            return Blacklist.Contains(player);
        }

        public string GetList()
        {
            return Blacklist.ToString();
        }
    }
}
