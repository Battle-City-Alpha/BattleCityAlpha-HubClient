using hub_client.Cards;
using hub_client.Enums;
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
            string level;
            if (typeArray.Contains(CardType.Pendule))
                level = string.Format("◊{0}    {1}◊", card.LScale, card.RScale);
            else
            {
                level = "★";
                for (int i = 1; i < card.Level; i++)
                    level += "★";
                if (level.Length > 6)
                    level = level.Insert(6, Environment.NewLine);
            }
            tb_cardlevel.Text = level;
            tb_cardatkdef.Text = string.Format("{0}/{1}", card.Atk, card.Def);
            tb_cardattribute.Text = card.GetAttribute();
            tb_cardtype.Text = card.GetCardType();
            tb_carddesc.Text = card.Description;

            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", card.Id + ".jpg" });
        }
    }
}
