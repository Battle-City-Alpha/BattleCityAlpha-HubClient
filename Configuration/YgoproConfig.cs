using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public static void UpdateNickname(string nickname)
        {
            string[] lines = File.ReadAllLines(Path.Combine(FormExecution.path, "BattleCityAlpha", "system.conf"));

            int index = Array.FindIndex(lines, x => x.StartsWith("nickname"));
            lines[index] = "nickname = " + nickname;

            File.WriteAllLines(Path.Combine(FormExecution.path, "BattleCityAlpha", "system.conf"), lines);
        }
        public static void UpdateForced(bool forced)
        {
            string[] lines = File.ReadAllLines(Path.Combine(FormExecution.path, "BattleCityAlpha", "system.conf"));
            try
            {
                int index = Array.FindIndex(lines, x => x.StartsWith("forced"));
                lines[index] = "forced = " + Convert.ToInt32(forced);
            }
            catch (Exception)
            {
                List<string> newlines = lines.ToList();
                newlines.Add("forced = " + Convert.ToInt32(forced));
                lines = newlines.ToArray();
            }

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
