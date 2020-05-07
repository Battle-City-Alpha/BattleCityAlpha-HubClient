using hub_client.Enums;
using hub_client.Stuff;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace hub_client.Helpers
{
    public static class BoosterManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static Dictionary<PurchaseType, List<BoosterInfo>> Boosters;

        public static void LoadList()
        {
            Boosters = new Dictionary<PurchaseType, List<BoosterInfo>>();

            string[] files = Directory.GetFiles(Path.Combine(FormExecution.path, "Assets", "Booster"));
            foreach (string file in files)
            {
                try
                {
                    if (!file.Contains(".json"))
                        continue;
                    BoosterInfo infos = GetBoosterInfo(Path.GetFileNameWithoutExtension(file));
                    if (!Boosters.ContainsKey(infos.Type))
                        Boosters[infos.Type] = new List<BoosterInfo>();
                    Boosters[infos.Type].Add(infos);
                }
                catch (Exception ex)
                {
                    logger.Error("ERROR LOADING BOOSTERS - {0}", ex.ToString());
                }
            }

            BoosterInfosDateComparer comparer = new BoosterInfosDateComparer();
            foreach (PurchaseType type in Enum.GetValues(typeof(PurchaseType)))
                if (Boosters.ContainsKey(type))
                    Boosters[type].Sort(comparer);
        }

        public static BoosterInfo InitializeBooster(string name)
        {
            string[] Extension = name.Split('(');
            Extension = Extension[1].Split(')');
            string path = Path.Combine(FormExecution.path, "Assets", "Booster", Extension[0] + ".json");
            return JsonConvert.DeserializeObject<BoosterInfo>(File.ReadAllText(path));
        }

        public static BoosterInfo GetBoosterInfo(string tag)
        {
            string path = Path.Combine(FormExecution.path, "Assets", "Booster", tag + ".json");
            return JsonConvert.DeserializeObject<BoosterInfo>(File.ReadAllText(path));
        }
    }
    public class BoosterInfosDateComparer : IComparer<BoosterInfo>
    {
        public int Compare(BoosterInfo x, BoosterInfo y)
        {
            try
            {
                CultureInfo frenchCI = new CultureInfo("fr-FR");
                DateTime xt = DateTime.Parse(x.Date, frenchCI);
                DateTime yt = DateTime.Parse(y.Date, frenchCI);
                return xt.CompareTo(yt);
            }
            catch
            {
                return 1;
            }
        }
    }
}