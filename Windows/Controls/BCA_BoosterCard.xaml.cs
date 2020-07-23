using BCA.Common;
using hub_client.Cards;
using hub_client.Enums;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_BoosterCard.xaml
    /// </summary>
    public partial class BCA_BoosterCard : UserControl
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        PlayerCard _card;
        public BCA_BoosterCard(PlayerCard card, bool isNew)
        {
            InitializeComponent();

            _card = card;
            lbl_quantity.Content = "x" + card.Quantity;

            if (!isNew)
                bd_new.Visibility = Visibility.Hidden;
            else
                bd_new.Visibility = Visibility.Visible;

            img_card.PreviewMouseWheel += Img_PreviewMouseWheel;
            img_card.MouseEnter += Img_MouseEnter;
            img_card.MouseLeave += Img_MouseLeave;
        }
        private void Img_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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

        private void Img_MouseLeave(object sender, MouseEventArgs e)
        {
            this.cardinfos_popup.IsOpen = false;
        }

        private void Img_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                SetCard(CardManager.GetCard(_card.Id));
                this.cardinfos_popup.IsOpen = true;
            }
            catch (Exception ex)
            {
                logger.Error("POPUP PURCHASE - {0}", ex.ToString());
            }
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

        public void SetImageSource(BitmapImage img)
        {
            img_card.Source = img;
        }
    }
}
