﻿using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;

namespace hub_client
{
    class Main
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static int CLIENT_VERSION = 2258;
        public static string VERSION = "2.2.5.8";

        public Main()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => FormExecution.InitConfig());
                if (!CheckClientUpdate())
                    Application.Current.Dispatcher.Invoke(() => FormExecution.Init());
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                FormExecution.Client_PopMessageBoxShowDialog("Une erreur s'est produite.", "Problème");
            }
        }

        public static bool CheckClientUpdate()
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
                    if (FormExecution.GetLastVersion(updatefilelines) != CLIENT_VERSION)
                    {
                        UpdateClient(updatefilelines);
                        return true;
                    }
                    else
                        return false;
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
            //FormExecution.HideLogin();
            FormExecution.Client_PopMessageBoxShowDialog("Un mise à jour du jeu est disponible !", "Mise à jour");
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
                LastVersion = FormExecution.GetLastVersion(updatefilelines).ToString(),
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
