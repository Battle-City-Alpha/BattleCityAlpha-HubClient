using BCA.Common.Enums;
using hub_client.Cards;
using hub_client.Configuration;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour BonusBox.xaml
    /// </summary>
    public partial class BonusBox : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        public BonusBox(BonusType type, int nbconnexions, string gift)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            LoadStyle();
            string bonus = "Bonus : ";
            switch (type)
            {
                case BonusType.BP:
                    bonus += gift + " BPs !";
                    showBonus.Margin = new Thickness(40);
                    break;
                case BonusType.PP:
                    bonus += gift + " PPs !";
                    showBonus.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Assets/Logo/CoinPP.png"));
                    showBonus.Margin = new Thickness(40);
                    break;
                case BonusType.Avatar:
                    bonus += "L'avatar n°" + gift + " !";
                    showBonus.Height = 256;
                    showBonus.Width = 256;
                    showBonus.Source = FormExecution.AssetsManager.GetImage("Avatars", Convert.ToInt32(gift).ToString());
                    break;
                case BonusType.Sleeve:
                    bonus += "La sleeve n°" + gift + " !";
                    break;
                case BonusType.Border:
                    bonus += "La bordure n°" + gift + " !";
                    break;
                case BonusType.Title:
                    bonus += "Le titre " + gift + " !";
                    break;
                case BonusType.Card:
                    bonus += "La carte " + CardManager.GetCard(Convert.ToInt32(gift)).Name + " !";
                    showBonus.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", gift + ".jpg" });
                    break;
                case BonusType.Booster:
                    bonus += "Un booster " + gift + " !";
                    showBonus.Source = FormExecution.AssetsManager.GetImage(new string[] { "Booster", "pics", gift + ".jpg" });
                    break;
            }
            bonus += Environment.NewLine + Environment.NewLine + "Reviens demain pour un nouveau bonus !";

            this.popText.Text = nbconnexions.ToString() + " connexions durant ce mois !" + Environment.NewLine + Environment.NewLine + bonus;

        }

        private void btnAgree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        public void LoadStyle()
        {
            btnAgree.Color1 = style.GetGameColor("Color1PopBoxButton");
            btnAgree.Color2 = style.GetGameColor("Color2PopBoxButton");
            btnAgree.Update();

            this.FontFamily = style.Font;
        }
    }
}
