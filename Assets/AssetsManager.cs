using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Assets
{
    public class AssetsManager
    {
        private string path;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AssetsManager()
        {
            path = FormExecution.path;
        }

        public BitmapImage GetImage(string directory, string img)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(Path.Combine(path, "Assets", directory, img + ".png"));
            image.EndInit();
            return image;
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
                return new BitmapImage(new Uri(Path.Combine(FormExecution.path,"BattleCityAlpha","textures","unknown.jpg")));
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
    }
}
