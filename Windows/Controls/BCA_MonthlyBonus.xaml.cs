using BCA.Common;
using BCA.Common.Enums;
using hub_client.Cards;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_MonthlyBonus.xaml
    /// </summary>
    public partial class BCA_MonthlyBonus : UserControl
    {
        BitmapImage colorpic;
        bool _isGray = false;
        public BCA_MonthlyBonus(MonthlyBonus bonus, int day, bool isGray, bool isToday = false)
        {
            InitializeComponent();
            _isGray = isGray;
            try
            {
                switch (bonus.Type)
                {
                    case BonusType.BP:
                        tb_left.Text = bonus.Gift;
                        tb_right.Text = "BPs";
                        img_bonus.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Assets/Logo/BPLogo.png"));
                        break;
                    case BonusType.PP:
                        tb_left.Text = bonus.Gift;
                        tb_right.Text = "PPs";
                        img_bonus.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Assets/Logo/CoinPP.png"));
                        break;
                    case BonusType.Avatar:
                        tb_left.Text = "Avatar";
                        tb_right.Text = bonus.Gift;
                        img_bonus.Source = FormExecution.AssetsManager.GetCustom(new Customization(CustomizationType.Avatar, Convert.ToInt32(bonus.Gift), false, ""));
                        break;
                    case BonusType.Sleeve:
                        tb_left.Text = "Sleeve";
                        tb_right.Text = bonus.Gift;
                        img_bonus.Source = FormExecution.AssetsManager.GetCustom(new Customization(CustomizationType.Sleeve, Convert.ToInt32(bonus.Gift), false, ""));
                        break;
                    case BonusType.Border:
                        tb_left.Text = "Bordure";
                        tb_right.Text = bonus.Gift;
                        img_bonus.Source = FormExecution.AssetsManager.GetCustom(new Customization(CustomizationType.Border, Convert.ToInt32(bonus.Gift), false, ""));
                        break;
                    case BonusType.Partner:
                        tb_left.Text = "Partenaire";
                        tb_right.Text = bonus.Gift;
                        img_bonus.Source = FormExecution.AssetsManager.GetCustom(new Customization(CustomizationType.Partner, Convert.ToInt32(bonus.Gift), false, ""));
                        break;
                    case BonusType.Title:
                        tb_left.Text = "Titre";
                        tb_right.Text = bonus.Gift;
                        img_bonus.Source = FormExecution.AssetsManager.GetPics(new string[] { "Assets", "Logo", "title_bonus.png" });
                        break;
                    case BonusType.Card:
                        tb_left.Text = "Carte";
                        try
                        {
                            tb_right.Text = CardManager.GetCard(Convert.ToInt32(bonus.Gift)).Name;
                        }
                        catch { tb_right.Text = bonus.Gift; }
                        img_bonus.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", bonus.Gift + ".jpg" });
                        break;
                    case BonusType.Booster:
                        tb_left.Text = "Booster(s)";
                        tb_right.Text = bonus.Gift;
                        img_bonus.Source = FormExecution.AssetsManager.GetImage(new string[] { "Booster", "pics", bonus.Gift + ".png" });
                        break;
                }
            }
            catch (Exception)
            {
                img_bonus.Source = FormExecution.AssetsManager.GetUnknownCardPic();
            }

            tb_day.Text = day.ToString();

            colorpic = img_bonus.Source as BitmapImage;
            if (isGray)
                img_bonus.Source = GetGrayScalePic();

            if (isToday)
            {
                bg_border.BorderThickness = new Thickness(5);
                bg_border.BorderBrush = new SolidColorBrush(Colors.Red);
            }

            img_bonus.MouseEnter += Img_bonus_MouseEnter;
            img_bonus.MouseLeave += Img_bonus_MouseLeave;
        }

        private void Img_bonus_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isGray)
                return;
            img_bonus.Source = GetGrayScalePic();
        }

        private void Img_bonus_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_isGray)
                return;
            img_bonus.Source = colorpic;
        }

        private FormatConvertedBitmap GetGrayScalePic()
        {
            BitmapImage img = new BitmapImage(new Uri(img_bonus.Source.ToString()));

            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            newFormatedBitmapSource.Source = img;
            newFormatedBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
            newFormatedBitmapSource.EndInit();

            return newFormatedBitmapSource;
        }
    }
}
