using hub_client.Cards;
using hub_client.Enums;
using hub_client.Stuff;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_CollectionCard.xaml
    /// </summary>
    public partial class BCA_CollectionCard : UserControl
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private CollectionCardItem _item;
        public BCA_CollectionCard(CollectionCardItem item)
        {
            InitializeComponent();

            _item = item;
            SetPic();
            this.tb_quantity.Text = _item.Quantity.ToString();

            if (_item.Rarity != 0)
                this.tb_rarity.Text = _item.Rarity.ToString().Replace('_', ' ');
            else
                this.tb_rarity.Visibility = Visibility.Hidden;

            img_card.MouseEnter += Img_card_MouseEnter;
            img_card.MouseLeave += Img_card_MouseLeave;
            img_card.PreviewMouseWheel += Img_card_PreviewMouseWheel;

            img_card.MouseLeftButtonDown += Img_card_MouseLeftButtonDown;
        }

        private void Img_card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SetCard(CardManager.GetCard(_item.Id));
                this.cardinfos_popup.IsOpen = true;
            }
            catch (Exception ex)
            {
                logger.Error("POPUP PURCHASE - {0}", ex.ToString());
            }
        }

        private void Img_card_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scrollPos = scrCardDesc.VerticalOffset - e.Delta;
            if (scrollPos < 0)
                scrollPos = 0;
            if (scrollPos > scrCardDesc.ScrollableHeight)
                scrollPos = (int)scrCardDesc.ScrollableHeight;

            if (this.cardinfos_popup.IsOpen)
            {
                scrCardDesc.ScrollToVerticalOffset(scrollPos);
                e.Handled = true;
            }
        }

        private void SetPic(bool hovered = false)
        {
            if (_item.Quantity == 0 && !hovered)
            {
                bg_border.BorderThickness = new Thickness(3);
                bg_border.BorderBrush = new SolidColorBrush(Colors.Red);
                bg_border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDA6A6"));
                img_card.Source = GetGrayScalePic();
            }
            else
                img_card.Source = GetPic();
        }
        private BitmapImage GetPic()
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(Path.Combine(FormExecution.path, "BattleCityAlpha", "pics", _item.Id + ".jpg"));
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
                return FormExecution.AssetsManager.GetUnknownCardPic();
            }
        }

        private void Img_card_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_item.Quantity == 0)
                img_card.Source = GetGrayScalePic();
            this.cardinfos_popup.IsOpen = false;
        }

        private void Img_card_MouseEnter(object sender, MouseEventArgs e)
        {
            //SetPic(true);     
        }

        private FormatConvertedBitmap GetGrayScalePic()
        {
            BitmapImage img = GetPic();

            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            newFormatedBitmapSource.Source = img;
            newFormatedBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
            newFormatedBitmapSource.EndInit();

            return newFormatedBitmapSource;
        }
        private void SetCard(CardInfos card)
        {
            tb_cardname.Text = card.Name;
            CardType[] typeArray = card.GetCardTypes();
            string level = "";
            string atkdef = "";
            string attribute = "";
            if (typeArray.Contains(CardType.Magie) || typeArray.Contains(CardType.Piège))
            {

            }
            else
            {
                if (!typeArray.Contains(CardType.Link))
                {
                    if (typeArray.Contains(CardType.Pendule))
                        level = string.Format("◊{0}    {1}◊", card.LScale, card.RScale);
                    else
                        level = card.Level + "★";

                    atkdef = string.Format("{0}/{1}", card.Atk, card.Def);
                }
                else
                {
                    LinkMarker[] markers = card.GetLinkMarkers();
                    atkdef = card.Atk + "/LINK-" + markers.Count();

                    level = GetStringLinksMarkers(markers);
                }
                attribute = string.Format("{0}|{1}", card.GetRace(), card.GetAttribute());
            }
            tb_cardlevel.Text = level;
            tb_cardatkdef.Text = atkdef;
            tb_cardattribute.Text = attribute;
            tb_cardtype.Text = card.GetCardType();
            tb_carddesc.Text = card.Description;

        }
        private string GetStringLinksMarkers(IEnumerable<LinkMarker> types)
        {
            string toReturn = "";
            foreach (var linkmarker in types)
            {
                switch (linkmarker)
                {
                    case LinkMarker.BottomLeft:
                        toReturn += "[↙]";
                        break;
                    case LinkMarker.Bottom:
                        toReturn += "[↓]";
                        break;
                    case LinkMarker.BottomRight:
                        toReturn += "[↘]";
                        break;
                    case LinkMarker.Left:
                        toReturn += "[←]";
                        break;
                    case LinkMarker.Right:
                        toReturn += "[→]";
                        break;
                    case LinkMarker.TopLeft:
                        toReturn += "[↖]";
                        break;
                    case LinkMarker.Top:
                        toReturn += "[↑]";
                        break;
                    case LinkMarker.TopRight:
                        toReturn += "[↗]";
                        break;
                }
            }
            return toReturn;
        }
    }
}
