using BCA.Common;
using BCA.Common.Enums;
using hub_client.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_MonthlyBonus.xaml
    /// </summary>
    public partial class BCA_MonthlyBonus : UserControl
    {
        public BCA_MonthlyBonus(MonthlyBonus bonus, int day, bool isGray, bool isToday = false)
        {
            InitializeComponent();

            switch (bonus.Type)
            {
                case BonusType.BP:
                    tb_left.Text = bonus.Gift;
                    tb_right.Text = "BPs";
                    break;
                case BonusType.PP:
                    tb_left.Text = bonus.Gift;
                    tb_right.Text = "PPs";
                    img_bonus.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Assets/Logo/CoinPP.png"));
                    break;
                case BonusType.Avatar:
                    tb_left.Text = "Avatar";
                    tb_right.Text = bonus.Gift;
                    img_bonus.Source = FormExecution.AssetsManager.GetImage("Avatars", Convert.ToInt32(bonus.Gift).ToString());
                    break;
                case BonusType.Sleeve:
                    tb_left.Text = "Sleeve";
                    tb_right.Text = bonus.Gift;
                    img_bonus.Source = FormExecution.AssetsManager.GetImage("Sleeves", Convert.ToInt32(bonus.Gift).ToString());
                    break;
                case BonusType.Border:
                    tb_left.Text = "Bordure";
                    tb_right.Text = bonus.Gift;
                    img_bonus.Source = FormExecution.AssetsManager.GetImage("Borders", Convert.ToInt32(bonus.Gift).ToString());
                    break;
                case BonusType.Title:
                    tb_left.Text = "Titre";
                    tb_right.Text = bonus.Gift;
                    img_bonus.Source = FormExecution.AssetsManager.GetPics(new string[] { "Assets", "Logo", "title_bonus.png" });
                    break;
                case BonusType.Card:
                    tb_left.Text = "Carte";
                    tb_right.Text = CardManager.GetCard(Convert.ToInt32(bonus.Gift)).Name;
                    img_bonus.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", bonus.Gift + ".jpg" });
                    break;
                case BonusType.Booster:
                    tb_left.Text = "Booster(s)";
                    tb_right.Text = bonus.Gift;
                    img_bonus.Source = FormExecution.AssetsManager.GetImage(new string[] { "Booster", "pics", bonus.Gift + ".jpg" });
                    break;
            }

            tb_day.Text = day.ToString();

            if (isGray)
                img_bonus.Source = GetGrayScalePic();

            if (isToday)
            {
                bg_border.BorderThickness = new Thickness(5);
                bg_border.BorderBrush = new SolidColorBrush(Colors.Red);
            }
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
