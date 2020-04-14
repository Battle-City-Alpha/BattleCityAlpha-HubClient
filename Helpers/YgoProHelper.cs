using BCA.Common;
using BCA.Common.Enums;
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
        private static string _deck = "";
        private static bool _botGame = false;
        private const int _defaultPort = 1111;
        private const string _defaultHost = "127.0.0.1";

        private static int[] _avatarsLoaded;

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

        public static void LaunchGame(Room room, string commandline)
        {
            _commandline = commandline;
            SetCustomization(room);
        }
        public static void LaunchGameAgainstBot(string deck)
        {
            _deck = deck;
            _botGame = true;
            Customization[] avatars = new Customization[2] { new Customization(CustomizationType.Avatar, 5000, true, "https://puu.sh/FxnII/10f3a11e50.png"), _bot_avatar };
            UpdateAvatar(avatars);
        }

        private static void SetCustomization(Room room)
        {
            UpdateAvatar(room.Avatars);
        }
        private static void UpdateAvatar(Customization[] avatars)
        {
            _avatarsLoaded = new int[2] { 0, avatars.Count() };
            for(int i = 0; i < avatars.Count(); i++)
            {
                Customization avatar = avatars[i];
                if (!avatar.IsHost)
                    CopyAvatarToTexturesFolder(avatar, i);
                else
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFile(
                            new System.Uri(avatar.URL),
                            Path.Combine(FormExecution.path, "Assets", "Avatars", "temp.png")
                            );
                    }
                    CopyAvatarToTexturesFolder(avatar, i);
                }
            }
        }

        private static void CopyAvatarToTexturesFolder(Customization avatar, int index)
        {
            if (avatar.IsHost)
                File.Copy(Path.Combine(FormExecution.path, "Assets", "Avatars", "temp.png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "Avatars", "a_" + index.ToString() + ".png"), true);
            else
                File.Copy(Path.Combine(FormExecution.path, "Assets", "Avatars", avatar.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "Avatars", "a_" + index.ToString() + ".png"), true);
            _avatarsLoaded[0]++;
            CheckIfAvatarsLoaded();
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

        private static void CheckIfAvatarsLoaded()
        {
            if (_avatarsLoaded[0] != _avatarsLoaded[1])
                return;

            if (_botGame)
            {
                LaunchYgoPro(String.Format("-h {0} -p {1} -c", _defaultHost, _defaultPort));
                Thread.Sleep(5000);
                LaunchWindbot(_deck);
                _botGame = false;
            }
            else
                LaunchYgoPro(_commandline);
        }
    }
}
