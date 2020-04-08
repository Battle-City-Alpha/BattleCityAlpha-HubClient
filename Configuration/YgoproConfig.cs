using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Configuration
{
    public static class YgoproConfig
    {
        public static void UpdateDefaultDeck(string defaultdeck)
        {
            string[] lines = File.ReadAllLines(Path.Combine(FormExecution.path, "BattleCityAlpha", "system.conf"));

            int index = Array.FindIndex(lines, x => x.StartsWith("lastdeck"));
            lines[index] = "lastdeck = " + defaultdeck;

            File.WriteAllLines(Path.Combine(FormExecution.path, "BattleCityAlpha", "system.conf"), lines);
        }

        public static string GetDefaultDeck()
        {
            string[] lines = File.ReadAllLines(Path.Combine(FormExecution.path, "BattleCityAlpha", "system.conf"));

            for (int i = 0; i < lines.Count(); i++)
                if (lines[i].StartsWith("lastdeck"))
                    return lines[i].Split('=')[1].Substring(1);
            return "Aucun";
        }
    }
}
