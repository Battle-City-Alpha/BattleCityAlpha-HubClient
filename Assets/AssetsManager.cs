using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Assets
{
    public class AssetsManager
    {
        private string path;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Dictionary<string, System.Windows.Controls.Image> Smileys;

        public AssetsManager()
        {
            path = FormExecution.path;

            LoadSmileys();
        }
        public System.Windows.Controls.Image CheckSmiley(string word)
        {
            if (!Smileys.ContainsKey(word))
                return null;
            return Smileys[word];
        }
        private void LoadSmileys()
        {
        Smileys = new Dictionary<string, System.Windows.Controls.Image>();
            List<string> smileys = new List<string>(Directory.EnumerateFiles(Path.Combine(FormExecution.path, "Assets", "smileys")));
            smileys.Sort();
            foreach (string smiley in smileys)
            {
                if (!smiley.EndsWith(".png"))
                    continue;
                string[] name = smiley.Split('\\');
                Smileys.Add(name[name.Length - 1].Split('.')[0], CreateSmileyImage(name[name.Length - 1]));
            }
        }
        private System.Windows.Controls.Image CreateSmileyImage(string name)
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = GetImage(new string[] { "Smileys", name});
            return image;
        }

        public BitmapImage GetImage(string directory, string img)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(Path.Combine(path, "Assets", directory, img + ".png"));
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
                logger.Error("IMAGE LOADING - {0}", ex);
                return null;
            }
        }

        public BitmapImage GetImage(string[] directory)
        {
            string img_path = Path.Combine(path, "Assets");
            foreach (string item in directory)
                img_path = Path.Combine(img_path, item);
            return new BitmapImage(new Uri(img_path));
        }

        public BitmapImage GetPics(string[] directory)
        {
            string img_path = FormExecution.path;
            foreach (string item in directory)
                img_path = Path.Combine(img_path, item);
            try
            {
                return new BitmapImage(new Uri(img_path));
            }
            catch (Exception ex)
            {
                logger.Warn("PICS LOADED:" + ex.ToString());
                return new BitmapImage(new Uri(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "unknown.png")));
            }
        }

        public Brush GetBrush(string directory, string img)
        {
            return new ImageBrush(new BitmapImage(new Uri(Path.Combine(path, "Assets", directory, img + ".png"))));
        }

        public Brush GetBrush(string directory, string img, string extension)
        {
            return new ImageBrush(new BitmapImage(new Uri(Path.Combine(path, "Assets", directory, img + "." + extension))));
        }

        public string GetSource(string directory, string img)
        {
            return Path.Combine(path, "Assets", directory, img);
        }

        public BitmapImage GetUnknownCardPic()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "unknown.jpg"));
            image.EndInit();
            return image;
        }
    }
}
