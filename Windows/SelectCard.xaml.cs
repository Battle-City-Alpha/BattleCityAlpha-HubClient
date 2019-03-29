using BCA.Common;
using hub_client.Cards;
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
    /// Logique d'interaction pour SelectCard.xaml
    /// </summary>
    public partial class SelectCard : Window
    {
        SelectCardAdministrator _admin;
        int selected_index = -1;

        public SelectCard(SelectCardAdministrator admin)
        {
            InitializeComponent();

            _admin = admin;
            _admin.LoadSelectCard += _admin_LoadSelectCard;

            Collection.GetListview().SelectionChanged += SelectCard_SelectionChanged;

            Closed += SelectCard_Closed;
        }

        private void SelectCard_Closed(object sender, EventArgs e)
        {
            _admin.LoadSelectCard -= _admin_LoadSelectCard;
            Collection.GetListview().SelectionChanged -= SelectCard_SelectionChanged;
        }

        private void _admin_LoadSelectCard(Dictionary<int, PlayerCard> cards)
        {
            Collection.UpdateCollection(cards);
        }

        private void SelectCard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Collection.SelectedItem() == null) return;
            DisplayCardInfo.SetCard(CardManager.GetCard(((PlayerCard)Collection.SelectedItem()).Id));
            selected_index = Collection.SelectedIndex();
        }
    }
}
