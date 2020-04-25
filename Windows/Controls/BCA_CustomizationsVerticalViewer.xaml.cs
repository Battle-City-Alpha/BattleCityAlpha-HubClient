using BCA.Common;
using hub_client.Assets;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_CustomizationsVerticalViewer.xaml
    /// </summary>
    public partial class BCA_CustomizationsVerticalViewer : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Customization[] _customs;
        private int _index = 2;

        AssetsManager PicsManager = new AssetsManager();
        public BCA_CustomizationsVerticalViewer()
        {
            InitializeComponent();
        }

        public void LoadFirstCustoms(Customization[] customs)
        {
            _customs = customs;

            LoadByIndex();
        }
        private void LoadByIndex()
        {
            if (_index > _customs.Length - 1)
                _index--;
            img_center.Source = LoadCustom(_customs[_index]);

            if (_index - 1 < 0)
                img_up.Source = null;
            else
                img_up.Source = LoadCustom(_customs[_index - 1]);

            if (_index + 1 > _customs.Length - 1)
                img_down.Source = null;
            else
                img_down.Source = LoadCustom(_customs[_index + 1]);
        }

        public void DownArrow()
        {
            if (_index + 1 > _customs.Length - 1)
                return;
            _index++;

            img_up.Source = img_center.Source;
            img_center.Source = img_down.Source;

            if (_index + 1 > _customs.Length - 1)
                img_down.Source = null;
            else
                img_down.Source = LoadCustom(_customs[_index + 1]);
        }
        public void UpArrow()
        {
            if (_index - 1 < 0)
                return;
            _index--;

            img_down.Source = img_center.Source;
            img_center.Source = img_up.Source;

            if (_index - 1 < 0)
                img_up.Source = null;
            else
                img_up.Source = LoadCustom(_customs[_index - 1]);
        }
        public int GetIndex()
        {
            return _index;
        }

        private BitmapImage LoadCustom(Customization custom)
        {
            if (!custom.IsHost)
                return GetImage("Borders", custom.Id.ToString());
            else
            {
                try { 
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(
                        new System.Uri(custom.URL),
                        Path.Combine(FormExecution.path, "Assets", "Borders", "temp.png")
                        );
                }
                return GetImage("Borders", "temp");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur", true);
                    return null;
                }
            }
        }
        private BitmapImage GetImage(string directory, string img)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(Path.Combine(FormExecution.path, "Assets", directory, img + ".png"));
            image.EndInit();
            return image;
        }
    }
}
