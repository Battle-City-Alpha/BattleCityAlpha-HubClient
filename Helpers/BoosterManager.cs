using hub_client.Stuff;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace hub_client.Helpers
{
    public static class BoosterManager
    {
        public static List<string> BoosterEditionList = new List<string>();
        public static List<string> BoosterPackList = new List<string>();
        public static List<string> ArsenalMysterieuxList = new List<string>();
        public static List<string> PackDuellisteList = new List<string>();
        public static List<string> DeckStructureList = new List<string>();
        public static List<string> DeckDeDemarrageList = new List<string>();
        public static List<string> GoldPremiumList = new List<string>();
        public static List<string> BattlePackList = new List<string>();
        public static List<string> BoosterSpecial = new List<string>();
        public static List<string> TournoiList = new List<string>();

        public static void LoadList()
        {
            BoosterEditionList.Clear();
            BoosterPackList.Clear();
            ArsenalMysterieuxList.Clear();
            PackDuellisteList.Clear();
            DeckStructureList.Clear();
            DeckDeDemarrageList.Clear();
            GoldPremiumList.Clear();
            BoosterSpecial.Clear();
            BattlePackList.Clear();
            TournoiList.Clear();

            BoosterEditionList.AddRange(File.ReadAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "booster_edition.bca")));
            BoosterPackList.AddRange(File.ReadAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "booster_pack.bca")));
            TournoiList.AddRange(File.ReadAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "tournoi_list.bca")));
            PackDuellisteList.AddRange(File.ReadAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "pack_du_duelliste.bca")));
            DeckStructureList.AddRange(File.ReadAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "struture_deck.bca")));
            DeckDeDemarrageList.AddRange(File.ReadAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "starter_deck.bca")));
            BattlePackList.AddRange(File.ReadAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "battle_pack.bca")));
            GoldPremiumList.AddRange(File.ReadAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "gold_premium.bca")));
            ArsenalMysterieuxList.AddRange(File.ReadAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "arsenal_mysterieux.bca")));
            BoosterSpecial.AddRange(File.ReadAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "booster_special.bca")));


            /*File.WriteAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "booster_edition.bca"), BoosterEditionList.ToArray());
            File.WriteAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "booster_pack.bca"), BoosterPackList.ToArray());
            File.WriteAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "tournoi_list.bca"), TournoiList.ToArray());
            File.WriteAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "pack_du_duelliste.bca"), PackDuellisteList.ToArray());
            File.WriteAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "struture_deck.bca"), DeckStructureList.ToArray());
            File.WriteAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "starter_deck.bca"), DeckDeDemarrageList.ToArray());
            File.WriteAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "battle_pack.bca"), BattlePackList.ToArray());
            File.WriteAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "gold_premium.bca"), GoldPremiumList.ToArray());
            File.WriteAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "arsenal_mysterieux.bca"), ArsenalMysterieuxList.ToArray());
            File.WriteAllLines(Path.Combine(FormExecution.path, "Assets", "Booster", "Lists", "booster_special.bca"), BoosterSpecial.ToArray());*/
        }

        public static BoosterInfo InitializeBooster(string name)
        {
            string[] Extension = name.Split('(');
            Extension = Extension[1].Split(')');
            string path = Path.Combine(FormExecution.path, "Assets", "Booster", Extension[0] + ".json");
            return JsonConvert.DeserializeObject<BoosterInfo>(File.ReadAllText(path));
        }
    }
}