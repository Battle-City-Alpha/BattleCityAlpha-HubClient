using BCA.Common;
using BCA.Common.Enums;
using hub_client.Assets;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_CustomizationsViewer.xaml
    /// </summary>
    public partial class BCA_CustomizationsViewer : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Customization[] _customs;
        private int _index = 2;
        string ctypetext = "";

        DoubleAnimation fadeInBorder;
        DoubleAnimation fadeOutBorder;
        DoubleAnimation fadeInText;
        DoubleAnimation fadeOutText;

        private Image[] imgs;

        AssetsManager PicsManager = FormExecution.AssetsManager;

        public BCA_CustomizationsViewer()
        {
            InitializeComponent();

            imgs = new Image[] { img_center, img_center_left, img_center_right, img_left, img_right };

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

            img_center.MouseEnter += Img_center_MouseEnter;
            img_center.MouseLeave += Img_center_MouseLeave;

            img_left.MouseLeftButtonDown += Img_left_MouseLeftButtonDown;
            img_right.MouseLeftButtonDown += Img_right_MouseLeftButtonDown;

            this.MouseWheel += BCA_CustomizationsViewer_MouseWheel;
        }

        private void BCA_CustomizationsViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                LeftArrow();
            else
                RightArrow();
        }

        private void Img_right_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RightArrow();
            RightArrow();
        }

        private void Img_left_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LeftArrow();
            LeftArrow();
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
            switch (_customs[0].CustomizationType)
            {
                case CustomizationType.Avatar:
                    ctypetext = "Avatars";

                    img_center.Width = 256;
                    img_center.Height = 256;
                    img_center_left.Width = 128;
                    img_center_left.Height = 128;
                    img_center_right.Width = 128;
                    img_center_right.Height = 128;
                    img_left.Width = 100;
                    img_left.Height = 100;
                    img_right.Width = 100;
                    img_right.Height = 100;
                    break;
                case CustomizationType.Partner:
                    ctypetext = "Partenaires";

                    img_center.Width = 256;
                    img_center.Height = 256;
                    img_center_left.Width = 128;
                    img_center_left.Height = 128;
                    img_center_right.Width = 128;
                    img_center_right.Height = 128;
                    img_left.Width = 100;
                    img_left.Height = 100;
                    img_right.Width = 100;
                    img_right.Height = 100;
                    break;
                case CustomizationType.Border:
                    ctypetext = "Borders";
                    break;
                case CustomizationType.Sleeve:
                    ctypetext = "Sleeves";

                    img_center.Width = 177;
                    img_center.Height = 254;
                    img_center_left.Width = 128;
                    img_center_left.Height = 184;
                    img_center_right.Width = 128;
                    img_center_right.Height = 184;
                    img_left.Width = 100;
                    img_left.Height = 144;
                    img_right.Width = 100;
                    img_right.Height = 144;
                    break;
            }

            LoadByIndex();
        }

        private void LoadByIndex()
        {

            if (_customs.Length - 1 < _index)
                _index--;
            img_center.Source = LoadCustom(_customs[_index]);

            if (_index - 1 < 0)
                img_center_left.Source = null;
            else
                img_center_left.Source = LoadCustom(_customs[_index - 1]);

            if (_index - 2 < 0)
                img_left.Source = null;
            else
                img_left.Source = LoadCustom(_customs[_index - 2]);

            if (_index + 1 > _customs.Length - 1)
                img_center_right.Source = null;
            else
                img_center_right.Source = LoadCustom(_customs[_index + 1]);

            if (_index + 2 > _customs.Length - 1)
                img_right.Source = null;
            else
                img_right.Source = LoadCustom(_customs[_index + 2]);
        }

        public void RightArrow()
        {
            if (_index + 1 > _customs.Length - 1)
                return;
            _index++;

            img_left.Source = img_center_left.Source;
            img_center_left.Source = img_center.Source;
            img_center.Source = img_center_right.Source;
            img_center_right.Source = img_right.Source;

            if (_index + 2 > _customs.Length - 1)
                img_right.Source = null;
            else
                img_right.Source = LoadCustom(_customs[_index + 2]);
        }
        public void LeftArrow()
        {
            if (_index - 1 < 0)
                return;
            _index--;

            img_right.Source = img_center_right.Source;
            img_center_right.Source = img_center.Source;
            img_center.Source = img_center_left.Source;
            img_center_left.Source = img_left.Source;

            if (_index - 2 < 0)
                img_left.Source = null;
            else
                img_left.Source = LoadCustom(_customs[_index - 2]);
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

    public enum CustomPos
    {
        Center,
        CenterLeft,
        CenterRight,
        Left,
        Right
    }
}
