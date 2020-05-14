using BCA.Common.Enums;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Stuff;
using hub_client.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour CollectionViewer.xaml
    /// </summary>
    public partial class CollectionViewer : Window
    {
        private event Action<BCA_CollectionCard> DisplayCard;
        private List<CollectionCardItem> _cards;

        public CollectionViewer(BoosterInfo info, List<int> ids, List<int> quantities, List<CardRarity> rarities)
        {
            InitializeComponent();
            LoadStyle();

            this.label_booster_name.Content = "[" + info.PurchaseTag + "] " + info.Name;
            this.total_count.Content = ids.Count();

            int numberIHave = info.CardsNumber;

            _cards = new List<CollectionCardItem>();
            for (int i = 0; i < ids.Count; i++)
            {
                if (quantities[i] == 0)
                    if (numberIHave != 0)
                        numberIHave--;
                _cards.Add(CreateCollectionCardItem(ids[i], quantities[i], rarities[i]));
            }
            _cards.Sort(new CollectionCardComparer());

            this.player_count.Content = numberIHave;

            DisplayCard += CollectionViewer_DisplayCard;
            this.MouseDown += Window_MouseDown;
        }

        private void CollectionViewer_DisplayCard(BCA_CollectionCard card)
        {
            wp_cards.Children.Add(card);
        }

        private CollectionCardItem CreateCollectionCardItem(int id, int qty, CardRarity rarity)
        {
            return new CollectionCardItem
            {
                Id = id,
                Quantity = qty,
                Rarity = rarity
            };
        }

        public void LoadCard()
        {
            foreach (CollectionCardItem item in _cards)
                Application.Current.Dispatcher.Invoke(() => DisplayCard?.Invoke(new BCA_CollectionCard(item)));
        }

        private void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;
            this.FontFamily = style.Font;
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.ActivateShop();
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(0, 50, 50, 0);
            }
            else if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.bg_border.CornerRadius = new CornerRadius(0);
            }
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
    public class CollectionCardComparer : IComparer<CollectionCardItem>
    {
        public int Compare(CollectionCardItem x, CollectionCardItem y)
        {
            if (x.Rarity != y.Rarity)
                return y.Rarity.CompareTo(x.Rarity);

            CardInfos xInfo = CardManager.GetCard(x.Id);
            CardInfos yInfo = CardManager.GetCard(y.Id);

            if (yInfo.Level != xInfo.Level)
                return yInfo.Level.CompareTo(xInfo.Level);

            if (xInfo.IsMonster() && yInfo.IsMonster())
                return yInfo.Name.CompareTo(xInfo.Name);
            if (xInfo.IsMonster() && yInfo.IsTrap())
                return 0;
            if (xInfo.IsMonster() && yInfo.IsSpell())
                return 0;

            if (xInfo.IsTrap() && yInfo.IsTrap())
                return yInfo.Name.CompareTo(xInfo.Name);
            if (xInfo.IsTrap() && yInfo.IsSpell())
                return 1;
            if (xInfo.IsTrap() && yInfo.IsMonster())
                return 1;

            if (xInfo.IsSpell() && yInfo.IsSpell())
                return yInfo.Name.CompareTo(xInfo.Name);
            if (xInfo.IsSpell() && yInfo.IsTrap())
                return 0;
            if (xInfo.IsSpell() && yInfo.IsMonster())
                return 1;

            return yInfo.Name.CompareTo(xInfo.Name);
            
        }
    }
}
