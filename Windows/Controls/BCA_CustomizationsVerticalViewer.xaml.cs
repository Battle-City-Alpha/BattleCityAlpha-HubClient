using BCA.Common;
using hub_client.Assets;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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
        private int _index = 1;

        DoubleAnimation fadeInBorder;
        DoubleAnimation fadeOutBorder;
        DoubleAnimation fadeInText;
        DoubleAnimation fadeOutText;

        AssetsManager PicsManager = new AssetsManager();
        public BCA_CustomizationsVerticalViewer()
        {
            InitializeComponent();

            img_center.MouseEnter += Img_center_MouseEnter;
            img_center.MouseLeave += Img_center_MouseLeave;

            fadeInBorder = new DoubleAnimation();
            fadeInBorder.From = 0.3;
            fadeInBorder.To = 1;
            fadeInBorder.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            fadeOutBorder = new DoubleAnimation();
            fadeOutBorder.From = 1;
            fadeOutBorder.To = 0.3;
            fadeOutBorder.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            fadeInText = new DoubleAnimation();
            fadeInText.From = 0;
            fadeInText.To = 1;
            fadeInText.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            fadeOutText = new DoubleAnimation();
            fadeOutText.From = 1;
            fadeOutText.To = 0;
            fadeOutText.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            this.MouseWheel += BCA_CustomizationsVerticalViewer_MouseWheel;
        }

        private void BCA_CustomizationsVerticalViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                UpArrow();
            else
                DownArrow();
        }

        private void Img_center_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tb_customID.BeginAnimation(OpacityProperty, fadeOutText);
            img_center.BeginAnimation(OpacityProperty, fadeInBorder);
        }

        private void Img_center_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                tb_customID.BeginAnimation(OpacityProperty, fadeInText);
                img_center.BeginAnimation(OpacityProperty, fadeOutBorder);
                tb_customID.Text = _customs[_index].Id.ToString();
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
            }
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
            return PicsManager.GetCustom(custom);
        }
    }
}
