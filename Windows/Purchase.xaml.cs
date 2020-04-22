using hub_client.Cards;
using hub_client.Enums;
using hub_client.Helpers;
using hub_client.Stuff;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.IO;
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
        private BoosterInfo _infos;

        public Purchase(PurchaseAdministrator admin, BoosterInfo infos)
        {
            InitializeComponent();
            _admin = admin;
            _admin.PurchaseItem += _admin_PurchaseItem;

            this.FontFamily = FormExecution.AppDesignConfig.Font;
            this.MouseDown += Window_MouseDown;

            _infos = infos;
        }

        public void UpdateCards(int[] list)
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

            lb_cards.Items.SortDescriptions.Add(
            new System.ComponentModel.SortDescription("",
            System.ComponentModel.ListSortDirection.Ascending));
        }

        private void SaveDeck(bool isStructureOrStartingDeck)
        {
            List<int> main = new List<int>();
            List<int> extra = new List<int>();

            foreach (CardInfos card in cards)
            {
                if (card.GetCardTypes().Contains(CardType.Synchro) || card.GetCardTypes().Contains(CardType.Xyz) || card.GetCardTypes().Contains(CardType.Fusion) || card.GetCardTypes().Contains(CardType.Link))
                {
                    if (!extra.Contains(card.Id))
                        extra.Add(card.Id);
                }
                else
                {
                    if (!main.Contains(card.Id))
                        main.Add(card.Id);
                }
            }

            string deck = "#created by purchase " + _infos.PurchaseTag;
            deck += Environment.NewLine + "#main";
            foreach (int id in main)
                deck += Environment.NewLine + id.ToString();
            deck += Environment.NewLine + "#extra";
            foreach (int id in extra)
                deck += Environment.NewLine + id.ToString();
            deck += Environment.NewLine + "!side";

            if (isStructureOrStartingDeck)
                File.WriteAllText(System.IO.Path.Combine(FormExecution.path, "BattleCityAlpha", "deck", _infos.Name + ".ydk"), deck);
            else
                File.WriteAllText(System.IO.Path.Combine(FormExecution.path, "BattleCityAlpha", "deck", "new_cards.ydk"), deck);
        }

        private void _admin_PurchaseItem(int[] list)
        {
            UpdateCards(list);
            SaveDeck((_infos.Type == PurchaseType.Demarrage || _infos.Type == PurchaseType.Structure));
        }

        private void lb_cards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CardInfos infos = cards[lb_cards.SelectedIndex];
            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", infos.Id.ToString() + ".jpg" });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.PurchaseItem -= _admin_PurchaseItem;
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void minimizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }
    }
}
