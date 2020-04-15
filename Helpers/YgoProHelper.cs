using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Standard.FromServer;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Helpers
{
    public static class YgoProHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static Customization _bot_avatar = new Customization(CustomizationType.Avatar, 9999, false, "");
        private static Customization _bot_border = new Customization(CustomizationType.Border, 5, false, "");
        private static Customization _bot_sleeve = new Customization(CustomizationType.Sleeve, 2, false, "");
        private static string _deck = "";
        private const int _defaultPort = 1111;
        private const string _defaultHost = "127.0.0.1";

        private static string _commandline = "";

        public static void ScaleBorder(int bid)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "Borders", "b_" + bid.ToString() + ".png"));
            image.EndInit();
            TransformedBitmap transformBmp = new TransformedBitmap(); 
            transformBmp.BeginInit();
            transformBmp.Source = image;
            transformBmp.Transform = new ScaleTransform(-1, 1);
            transformBmp.EndInit();

            using (var fileStream = new FileStream(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "Borders", "b_" + bid.ToString() + ".png"), FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(transformBmp));
                encoder.Save(fileStream);
            }
        }

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

            LaunchYgoPro(String.Format("-h {0} -p {1} -c", _defaultHost, _defaultPort));
            Thread.Sleep(5000);
            LaunchWindbot(_deck);
        }

        public static void LoadCustomization(Customization avatar, Customization border, Customization sleeve, int pos)
        {
            UpdateAvatar(avatar, pos);
            UpdateBorder(border, pos);
            UpdateSleeves(sleeve, pos);
        }
        private static void UpdateAvatar(Customization avatar, int i)
        {
                if (!avatar.IsHost)
                    CopyAvatarToTexturesFolder(avatar, i);
                else
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileAsync(
                            new System.Uri(avatar.URL),
                            Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "avatars", "a_" + i + ".png")
                            );
                        wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, avatar, i);
                    }
                }
        }
        private static void UpdateBorder(Customization border, int i)
        {
                if (!border.IsHost)
                    CopyBorderToTexturesFolder(border, i);
                else
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileAsync(
                            new System.Uri(border.URL),
                            Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "borders", "b_" + i + ".png")
                            );
                        wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, border, i);
                    }
                }
        }
        private static void UpdateSleeves(Customization sleeve, int i)
        {
                if (!sleeve.IsHost)
                    CopySleeveToTexturesFolder(sleeve, i);
                else
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileAsync(
                            new System.Uri(sleeve.URL),
                            Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "sleeves", "s_" + i + ".png")
                            );
                        wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, sleeve, i);
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

            if (index % 2 != 0)
                ScaleBorder(index);
        }
        private static void CopySleeveToTexturesFolder(Customization sleeve, int index)
        {
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Sleeves", sleeve.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "sleeves", "s_" + index.ToString() + ".png"), true);
        }
    }
}
