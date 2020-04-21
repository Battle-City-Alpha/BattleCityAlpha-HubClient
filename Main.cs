using hub_client.Cards;
using hub_client.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace hub_client
{
    class Main
    {
        public static string VERSION = "2.0.0.0";

        public Main()
        {
            FormExecution.Init();
            VERSION += "c" + FormExecution.ClientConfig.CardsStuffVersion;

            CheckCardsStuffUpdate();
        }

        private void CheckCardsStuffUpdate()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string query = "https://battlecityalpha.xyz/BCA/UPDATEV2/CardsStuff/update.txt";
                    string updateCardsStuff = wc.DownloadString(query);
                    if (GetLastVersion(updateCardsStuff) != FormExecution.ClientConfig.CardsStuffVersion)
                        UpdateCardsStuff(updateCardsStuff);
                    else
                        return;
                }
            }
            catch { return; }
        }
        private int GetLastVersion(string updatefile)
        {
            return Convert.ToInt32(updatefile.Split(' ')[0]);
        }
        private void UpdateCardsStuff(string updatefile)
        {
            FormExecution.Client_PopMessageBox("Un mise à jour au niveau des cartes et des boosters est disponible !", "Mise à jour", true);

            string[] updates = updatefile.Split(' ');
            List<string> updatesToDo = new List<string>();

            int i = 0;
            while (updates[i] != FormExecution.ClientConfig.CardsStuffVersion.ToString())
            {
                updatesToDo.Add(updates[i]);
                i++;
            }

            UpdateCardsStuffWindow window = new UpdateCardsStuffWindow(updatesToDo.ToArray());
            window.Show();

            FormExecution.ClientConfig.CardsStuffVersion = Convert.ToInt32(updatesToDo.Last());
            FormExecution.SaveConfig();
            CardManager.LoadCDB(Path.Combine(FormExecution.path, "BattleCityAlpha", "cards.cdb"), true, true);
        }
    }
}
