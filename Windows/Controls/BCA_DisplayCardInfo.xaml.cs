using hub_client.Cards;
using hub_client.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_DisplayCardInfo.xaml
    /// </summary>
    public partial class BCA_DisplayCardInfo : UserControl
    {
        public BCA_DisplayCardInfo()
        {
            InitializeComponent();
        }

        public void SetCard(CardInfos card)
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

            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", card.Id + ".jpg" });
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
