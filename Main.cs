using hub_client.Cards;
using hub_client.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
            FormExecution.HideLogin();

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

            FormExecution.ShowLogin();
        }

        private static bool CheckUpdate()
        {
            try
            {

                WebClient wc = new WebClient();
                string query = "http://battlecityalpha.xyz/BCA/MAJ/MAJ.php?version=13040";
                string text = wc.DownloadString(query);
                if (!text.Contains("A_Jour") && text != "Maj#")
                {
                    FormExecution.Client_PopMessageBox("Une mise à jour est disponible !", "Mise à jour", true);
                    int a = -1;
                    string Update = "#";
                    string[] data = text.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string Maj in data)
                    {
                        a = a + 1;
                        if (a == 0 || a == data.Length - 1)
                            continue;

                        Update += Maj + "#";

                    }

                    WebClient wc2 = new WebClient();
                    string lastVersion = wc2.DownloadString("http://battlecityalpha.xyz/BCA/MAJ/LastVersion.config");
                    WebClient wc3 = new WebClient();
                    string News = wc2.DownloadString("http://battlecityalpha.xyz/BCA/MAJ/News.config");
                    string arg = lastVersion + " " + Update + " " + Assembly.GetExecutingAssembly().Location;
                    Process p = new Process();
                    p.StartInfo.FileName = Path.Combine(FormExecution.path, "BCAUpdater.exe");
                    p.StartInfo.WorkingDirectory = Path.Combine(FormExecution.path);
                    p.StartInfo.Arguments = arg;
                    p.StartInfo.Verb = "runas";
                    p.Start();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                FormExecution.Client_PopMessageBox("Problème de connexion pour vérifier les mises à jour..", "Erreur mise à jour", true);
                return false;
            }
        }
    }
}
