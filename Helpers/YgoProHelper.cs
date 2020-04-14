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

        private static Customization _bot_avatar = new Customization(CustomizationType.Avatar, 9999, true, "https://puu.sh/FxnII/10f3a11e50.png");
        private static Customization _bot_border = new Customization(CustomizationType.Border, 5, true, "https://cdn.discordapp.com/attachments/435545676328468500/699629018248511538/Test8.png");
        private static Customization _bot_sleeve = new Customization(CustomizationType.Sleeve, 1, true, "https://puu.sh/Fxzry/1f84fc22a6.jpg");
        private static string _deck = "";
        private static bool _botGame = false;
        private const int _defaultPort = 1111;
        private const string _defaultHost = "127.0.0.1";

        private static int[] _avatarsLoaded;
        private static int[] _bordersLoaded;
        private static int[] _sleevesLoaded;

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
            _commandline = commandline;
            SetCustomization(room);
        }
        public static void LaunchGameAgainstBot(string deck)
        {
            _deck = deck;
            _botGame = true;

            _avatarsLoaded = new int[2] { 0, 2 };
            _bordersLoaded = new int[2] { 0, 2 };
            _sleevesLoaded = new int[2] { 0, 2 };

            Customization[] avatars = new Customization[2] { new Customization(CustomizationType.Avatar, 5000, true, "https://puu.sh/FxnII/10f3a11e50.png"), _bot_avatar };
            UpdateAvatar(avatars);
            Customization[] borders = new Customization[2] { new Customization(CustomizationType.Border, 1, true, "https://cdn.discordapp.com/attachments/435545676328468500/699413524245381130/Test3.png"), _bot_border };
            UpdateBorders(borders);
            Customization[] sleeves = new Customization[2] { _bot_sleeve, _bot_sleeve };
            UpdateSleeves(sleeves);
        }

        private static void SetCustomization(Room room)
        {
            _avatarsLoaded = new int[2] { 0, room.Avatars.Count() };
            _bordersLoaded = new int[2] { 0, room.Borders.Count() };
            _sleevesLoaded = new int[2] { 0, room.Sleeves.Count() };
            UpdateAvatar(room.Avatars);
            UpdateBorders(room.Borders);
            UpdateSleeves(room.Sleeves);
        }
        private static void UpdateAvatar(Customization[] avatars)
        {
            for(int i = 0; i < avatars.Count(); i++)
            {
                Customization avatar = avatars[i];
                if (!avatar.IsHost)
                    CopyAvatarToTexturesFolder(avatar, i);
                else
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileAsync(
                            new System.Uri(avatar.URL),
                            Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "avatars", "a_" + i.ToString() + ".png")
                            );
                        wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, avatar, i);
                    }
                }
            }
        }
        private static void UpdateBorders(Customization[] borders)
        {
            for (int i = 0; i < borders.Count(); i++)
            {
                Customization border = borders[i];
                if (!border.IsHost)
                    CopyBorderToTexturesFolder(border, i);
                else
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileAsync(
                            new System.Uri(border.URL),
                            Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "borders", "b_" + index.ToString() + ".png")
                            );
                        wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, border, i);
                    }
                }
            }
        }
        private static void UpdateSleeves(Customization[] sleeves)
        {
            for (int i = 0; i < sleeves.Count(); i++)
            {
                Customization sleeve = sleeves[i];
                if (!sleeve.IsHost)
                    CopySleeveToTexturesFolder(sleeve, i);
                else
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileAsync(
                            new System.Uri(sleeve.URL),
                            Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "sleeves", "s_" + i.ToString() + ".jpg")
                            );
                        wc.DownloadFileCompleted += (sender, e) => Wc_DownloadFileCompleted(sender, e, sleeve, i);
                    }
                }
            }
        }

        private static void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e, Customization customitem, int i)
        {
            switch(customitem.CustomizationType)
            {
                case CustomizationType.Avatar:
                    _avatarsLoaded[0]++;
                    break;
                case CustomizationType.Border:
                    _bordersLoaded[0]++;
                    break;
                case CustomizationType.Sleeve:
                    _sleevesLoaded[0]++;
                    break;
            }
            CheckIfTexturesAreLoaded();
        }

        private static void CopyAvatarToTexturesFolder(Customization avatar, int index)
        {
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Avatars", avatar.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "avatars", "a_" + index.ToString() + ".png"), true);
            _avatarsLoaded[0]++;
            CheckIfTexturesAreLoaded();
        }
        private static void CopyBorderToTexturesFolder(Customization border, int index)
        {
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Borders", border.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "borders", "b_" + index.ToString() + ".png"), true);

            if (index % 2 != 0)
                ScaleBorder(index);

            _bordersLoaded[0]++;
            CheckIfTexturesAreLoaded();
        }
        private static void CopySleeveToTexturesFolder(Customization sleeve, int index)
        {
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Sleeves", sleeve.Id.ToString() + ".jpg"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "sleeves", "s_" + index.ToString() + ".jpg"), true);
            _sleevesLoaded[0]++;
            CheckIfTexturesAreLoaded();
        }

        private static void CheckIfTexturesAreLoaded()
        {
            if ((_avatarsLoaded[0] != _avatarsLoaded[1]) || (_bordersLoaded[0] != _bordersLoaded[1]) || (_sleevesLoaded[0] != _sleevesLoaded[1]))
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
