using BCA.Common;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public event Action<PlayerCard, int, int> SelectedCard;

        public SelectCard(SelectCardAdministrator admin)
        {
            InitializeComponent();

            _admin = admin;
            _admin.LoadSelectCard += _admin_LoadSelectCard;

            Collection.GetListview().SelectionChanged += SelectCard_SelectionChanged;

            Closed += SelectCard_Closed;

            LoadStyle();
        }

        public void ActivateDonationCardMode()
        {
            this.tb_price.IsEnabled = false;
            this.tb_price.Text = "1";
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

        private void BCA_ColorButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Collection.SelectedItem() == null || tb_price.Text == string.Empty || Convert.ToInt32(tb_price.Text) <= 0 || tb_quantity.Text == string.Empty || Convert.ToInt32(tb_quantity.Text) <= 0) return;
            SelectedCard?.Invoke((PlayerCard)Collection.SelectedItem(), Convert.ToInt32(tb_price.Text), Convert.ToInt32(tb_quantity.Text));
            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;

            btnSelect.Color1 = style.Color1BrocanteButton;
            btnSelect.Color2 = style.Color2BrocanteButton;
            btnSelect.Update();
        }
    }
}
