using hub_client.Cards;
using hub_client.Windows;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace hub_client
{
    class Main
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private int CLIENT_VERSION = 2000;
        public static string VERSION = "2.0.0.0";

        public Main()
        {
            try
            {
                FormExecution.Init();
                VERSION += "c" + FormExecution.ClientConfig.CardsStuffVersion;

                CheckClientUpdate();
                CheckCardsStuffUpdate();
            }
            catch (Exception ex)
            {
                logger.Fatal("GLOBAL ERROR - {0}", ex);
                FormExecution.Client_PopMessageBox("Une erreur s'est produite.", "Problème", true);
                //Thread.Sleep(1500);
                Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
            }
        }

        private void CheckCardsStuffUpdate()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string query = "https://battlecityalpha.xyz/BCA/UPDATEV2/CardsStuff/updates.txt";
                    string updateCardsStuff = wc.DownloadString(query);
                    string[] updatefilelines = updateCardsStuff.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                    );
                    if (GetLastVersion(updatefilelines) != FormExecution.ClientConfig.CardsStuffVersion)
                        UpdateCardsStuff(updatefilelines);
                    else
                        return;
                }
            }
            catch { return; }
        }
        private int GetLastVersion(string[] updatefilelines)
        {
            return Convert.ToInt32(updatefilelines[0]);
        }
        private void UpdateCardsStuff(string[] updatefilelines)
        {
            FormExecution.HideLogin();

            FormExecution.Client_PopMessageBox("Un mise à jour au niveau des cartes et des boosters est disponible !", "Mise à jour", true);

            List<string> updatesToDo = new List<string>();

            int i = 0;
            while (updatefilelines[i] != FormExecution.ClientConfig.CardsStuffVersion.ToString() && i < updatefilelines.Length - 1)
            {
                updatesToDo.Add(updatefilelines[i]);
                i++;
            }

            UpdateCardsStuffWindow window = new UpdateCardsStuffWindow(updatesToDo.ToArray());
            window.Show();

            FormExecution.ClientConfig.CardsStuffVersion = Convert.ToInt32(updatesToDo.Last());
            FormExecution.SaveConfig();
            CardManager.LoadCDB(Path.Combine(FormExecution.path, "BattleCityAlpha", "cards.cdb"), true, true);

            FormExecution.ShowLogin();
        }
        private void CheckClientUpdate()
        {

            try
            {
                using (WebClient wc = new WebClient())
                {
                    string query = "https://battlecityalpha.xyz/BCA/UPDATEV2/Client/updates.txt";
                    string updateCardsStuff = wc.DownloadString(query);
                    string[] updatefilelines = updateCardsStuff.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                    );
                    if (GetLastVersion(updatefilelines) != CLIENT_VERSION)
                        UpdateClient(updatefilelines);
                    else
                        return;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ERROR SEEKING UPDATE - {0}", ex);
                FormExecution.Client_PopMessageBox("Problème de connexion pour vérifier les mises à jour..", "Erreur mise à jour", true);
                Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
                return;
            }
        }

        private void UpdateClient(string[] updatefilelines)
        {
            string updates = "#";
            List<string> updatesToDo = new List<string>();
            int i = 0;
            while (updatefilelines[i] != CLIENT_VERSION.ToString() && i < updatefilelines.Length - 1)
            {
                updatesToDo.Add(GetClientUpdateURL(updatefilelines[i]));
                i++;
            }

            updatesToDo.Reverse();

            foreach (string update in updatesToDo)
                updates += update + "#";

            string arg = GetLastVersion(updatefilelines) + " " + updates + " " + Assembly.GetExecutingAssembly().Location;

            Process p = new Process();
            p.StartInfo.FileName = Path.Combine(FormExecution.path, "BCAUpdater.exe");
            p.StartInfo.WorkingDirectory = Path.Combine(FormExecution.path);
            p.StartInfo.Arguments = arg;
            p.StartInfo.Verb = "runas";
            p.Start();
        }
        private string GetClientUpdateURL(string version)
        {
            return string.Format("https://battlecityalpha.xyz/BCA/UPDATEV2/Client/zip/{0}.zip", version);
        }
    }
}
