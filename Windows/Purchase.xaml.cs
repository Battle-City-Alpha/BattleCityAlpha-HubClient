using hub_client.Cards;
using hub_client.Helpers;
using hub_client.Stuff;
using hub_client.WindowsAdministrator;
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
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Purchase.xaml
    /// </summary>
    public partial class Purchase : Window
    {
        PurchaseAdministrator _admin;
        CardInfos[] cards;
        ToolTip tip = new ToolTip();

        public Purchase(PurchaseAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;
            _admin.PurchaseItem += _admin_PurchaseItem;            
        }

        private void _admin_PurchaseItem(int[] list)
        {
            cards = new CardInfos[list.Count()];
            for (int i = 0; i < list.Count(); i++)
            {
                CardInfos infos = CardManager.GetCard(list[i]);
                cards[i] = infos;
                if (infos == null)
                    infos.Name = "Carte inconnue. Id : " + list[i].ToString();
                lb_cards.Items.Add(infos.Name);
            }
        }

        private void lb_cards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CardInfos infos = cards[lb_cards.SelectedIndex];
            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", infos.Id.ToString() + ".jpg" });
        }
    }
}
