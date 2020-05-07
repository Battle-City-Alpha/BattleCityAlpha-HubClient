using hub_client.Cards;
using hub_client.Windows;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace hub_client
{
    class Main
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static int CLIENT_VERSION = 20014;
        public static string VERSION = "2.0.0.14";

        public Main()
        {
            try
            {
                FormExecution.Init();
                CheckClientUpdate();
                CheckCardsStuffUpdate();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                FormExecution.Client_PopMessageBox("Une erreur s'est produite.", "Problème", true);
            }
        }

        public static void CheckCardsStuffUpdate()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string query = "http://battlecityalpha.xyz/BCA/UPDATEV2/CardsStuff/updates.txt";
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
        private static int GetLastVersion(string[] updatefilelines)
        {
            return Convert.ToInt32(updatefilelines[0]);
        }
        private static void UpdateCardsStuff(string[] updatefilelines)
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
            window.Topmost = true;
            window.Show();

            FormExecution.ClientConfig.CardsStuffVersion = Convert.ToInt32(updatesToDo[0]);
            FormExecution.SaveConfig();
            CardManager.LoadCDB(Path.Combine(FormExecution.path, "BattleCityAlpha", "cards.cdb"), true, true);

            FormExecution.ShowLogin();
        }
        public static void CheckClientUpdate()
        {

            try
            {
                using (WebClient wc = new WebClient())
                {
                    string query = "http://battlecityalpha.xyz/BCA/UPDATEV2/Client/updates.txt";
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
                logger.Error(ex);
                throw ex;
            }
        }

        private static void UpdateClient(string[] updatefilelines)
        {
            FormExecution.HideLogin();
            FormExecution.Client_PopMessageBox("Un mise à jour du jeu est disponible !", "Mise à jour", true);
            List<string> updatesToDo = new List<string>();
            int i = 0;
            while (updatefilelines[i] != CLIENT_VERSION.ToString() && i < updatefilelines.Length - 1)
            {
                updatesToDo.Add(GetClientUpdateURL(updatefilelines[i]));
                i++;
            }

            updatesToDo.Reverse();

            UpdatesInfos infos = new UpdatesInfos
            {
                LastVersion = GetLastVersion(updatefilelines).ToString(),
                Updates = updatesToDo.ToArray(),
                ProcessName = Assembly.GetExecutingAssembly().Location
            };

            Process p = new Process();
            p.StartInfo.FileName = Path.Combine(FormExecution.path, "BattleCityAlpha-Updater.exe");
            p.StartInfo.WorkingDirectory = Path.Combine(FormExecution.path);
            string jsonStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(infos)));
            p.StartInfo.Arguments = jsonStr;
            p.StartInfo.Verb = "runas";
            p.Start();
        }
        private static string GetClientUpdateURL(string version)
        {
            return string.Format("http://battlecityalpha.xyz/BCA/UPDATEV2/Client/zip/{0}.zip", version);
        }
    }

    public class UpdatesInfos
    {
        public string LastVersion { get; set; }
        public string[] Updates { get; set; }
        public string ProcessName { get; set; }
    }
}
