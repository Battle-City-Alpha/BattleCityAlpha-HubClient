using BCA.Common;
using BCA.Common.Enums;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace hub_client.Helpers
{
    public static class YgoProHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static Customization _bot_avatar = new Customization(CustomizationType.Avatar, 406, false, "");
        private static Customization _bot_border = new Customization(CustomizationType.Border, 26, false, "");
        private static Customization _bot_sleeve = new Customization(CustomizationType.Sleeve, 64, false, "");
        private static string _deck = "";
        private const int _defaultPort = 1111;
        private const string _defaultHost = "127.0.0.1";

        private static string _commandline = "";

        public static void LaunchYgoPro(string commandline)
        {
            Process Game = new Process();
            Game.StartInfo.FileName = Path.Combine(FormExecution.path, "BattleCityAlpha", "BCA.exe");
            Game.StartInfo.WorkingDirectory = Path.Combine(FormExecution.path, "BattleCityAlpha");
            Game.StartInfo.Arguments = commandline;
            Game.Start();
        }
        private static void LaunchWindbot(string deck, string host = _defaultHost, int port = _defaultPort, int version = 0x1340, string dialog = "fr-FR", string name = "Kaibot")
        {
            string info = String.Format("Name={0}  Deck={1} Dialog={2} Host={3} Port={4} Version={5}", name, deck, dialog, host, port, version);

            logger.Trace("Windbot start with : " + info);

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = Path.Combine(FormExecution.path, "BattleCityAlpha", "Kaibot", "WindBot.exe");
            p.StartInfo.WorkingDirectory = Path.Combine(FormExecution.path, "BattleCityAlpha", "Kaibot");
            p.StartInfo.Arguments = info;
            p.Start();
        }

        public static void LaunchGame(Room room, string commandline)
        {
            LaunchYgoPro(commandline);
        }
        public static void LaunchGameAgainstBot(string deck)
        {
            _deck = deck;
            UpdateAvatar(new Customization(CustomizationType.Avatar, 14, false, ""), 0);
            UpdateBorder(new Customization(CustomizationType.Border, 1, false, ""), 0);
            UpdateSleeve(new Customization(CustomizationType.Sleeve, 203, false, ""), 0);
            UpdateAvatar(_bot_avatar, 1);
            UpdateBorder(_bot_border, 1);
            UpdateSleeve(_bot_sleeve, 1);

            LaunchYgoPro(String.Format("-h {0} -p {1} -c", _defaultHost, _defaultPort));
            Thread.Sleep(5000);
            LaunchWindbot(_deck);
        }

        public static void LoadCustomization(Customization avatar, Customization border, Customization sleeve, int pos)
        {
            UpdateAvatar(avatar, pos);
            UpdateBorder(border, pos);
            UpdateSleeve(sleeve, pos);
        }
        private static void UpdateAvatar(Customization avatar, int i)
        {
            if (!avatar.IsHost)
                CopyAvatarToTexturesFolder(avatar, i);
            else
            {
                try { 
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileAsync(
                        new System.Uri(avatar.URL),
                        Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "avatars", "a_" + i + ".png")
                        );
                    wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, avatar, i);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur", true);
                }
            }
        }
        private static void UpdateBorder(Customization border, int i)
        {
            if (!border.IsHost)
                CopyBorderToTexturesFolder(border, i);
            else
            {
                try { 
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileAsync(
                        new System.Uri(border.URL),
                        Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "borders", "b_" + i + ".png")
                        );
                    wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, border, i);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur", true);
                }
            }
        }
        private static void UpdateSleeve(Customization sleeve, int i)
        {
            if (!sleeve.IsHost)
                CopySleeveToTexturesFolder(sleeve, i);
            else
            {
                try { 
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileAsync(
                        new System.Uri(sleeve.URL),
                        Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "sleeves", "s_" + i + ".png")
                        );
                    wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, sleeve, i);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur", true);
                }
            }
        }

        private static void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e, Customization customitem, int i)
        {

        }

        private static void CopyAvatarToTexturesFolder(Customization avatar, int index)
        {
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Avatars", avatar.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "avatars", "a_" + index.ToString() + ".png"), true);
        }
        private static void CopyBorderToTexturesFolder(Customization border, int index)
        {
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Borders", border.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "borders", "b_" + index.ToString() + ".png"), true);
        }
        private static void CopySleeveToTexturesFolder(Customization sleeve, int index)
        {
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Sleeves", sleeve.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "sleeves", "s_" + index.ToString() + ".png"), true);
        }
    }
}
