using BCA.Common;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Brocante.xaml
    /// </summary>
    public partial class Brocante : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        BrocanteAdministrator _admin;

        List<BrocanteCard> _cards;
        bool searchMyCard = true;

        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        private DispatcherTimer _popupTimer;

        public Brocante(BrocanteAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _admin = admin;

            _admin.LoadBrocante += _admin_LoadBrocante;

            LoadStyle();

            _cards = new List<BrocanteCard>();

            _popupTimer = new DispatcherTimer();
            _popupTimer.Interval = TimeSpan.FromMilliseconds(1000);
            _popupTimer.Tick += _popupTimer_Tick;
            _popupTimer.IsEnabled = false;



            this.MouseDown += Window_MouseDown;
        }

        private void _popupTimer_Tick(object sender, EventArgs e)
        {
            brocante_reload_popup.IsOpen = false;
            _popupTimer.IsEnabled = false;
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> RankedButtons = new List<BCA_ColorButton>();
            RankedButtons.AddRange(new[] { btnSell, btnMyCards });

            AppDesignConfig style = FormExecution.AppDesignConfig;

            foreach (BCA_ColorButton btn in RankedButtons)
            {
                btn.Color1 = style.GetGameColor("Color1BrocanteButton");
                btn.Color2 = style.GetGameColor("Color2BrocanteButton");
                btn.Update();
            }

            btnBuy.Color1 = style.GetGameColor("Color1CenterBrocanteButton");
            btnBuy.Color2 = style.GetGameColor("Color2CenterBrocanteButton");
            btnBuy.Update();
            this.FontFamily = style.Font;
        }

        private void _admin_LoadBrocante(List<BrocanteCard> cards)
        {
            int sitem = brocanteList.SelectedIndex;

            brocanteList.ItemsSource = null;
            _cards = cards;

            foreach (BrocanteCard card in _cards)
                card.CardName = CardManager.GetCard(card.Id).Name;

            brocanteList.ItemsSource = _cards;

            //_admin.Client.OpenPopBox("Les cartes dans la brocante ont été mises à jour.", "Mise à jour brocante");

            if (this.IsActive)
            {
                this.brocante_reload_popup.IsOpen = true;
                _popupTimer.IsEnabled = true;
            }

            if (sitem != -1 && brocanteList.Items.Count > sitem)
                brocanteList.SelectedItem = sitem;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _admin.AskBrocante();

            tb_search_seller.SetText("Vendeur...");
            tb_search_card.SetText("Carte...");
            tb_search_max_price.SetText("Prix max...");
            tb__search_max_quantity.SetText("Quantité max...");

            tb_search_seller.tbChat.SelectionChanged += TbChat_Seller_SelectionChanged;
            tb_search_card.tbChat.SelectionChanged += TbChat_Card_SelectionChanged;
            tb_search_max_price.tbChat.SelectionChanged += TbChat_MaxPrice_SelectionChanged;
            tb__search_max_quantity.tbChat.SelectionChanged += TbChat_MaxQuantity_SelectionChanged;
            tb_search_max_price.PreviewTextInput += NumberValidationTextBox;
            tb__search_max_quantity.PreviewTextInput += NumberValidationTextBox;

            tb_search_card.tbChat.GotFocus += TbChat_GotFocus;
            tb__search_max_quantity.tbChat.GotFocus += TbChat_GotFocus;
            tb_search_max_price.tbChat.GotFocus += TbChat_GotFocus;
            tb_search_seller.tbChat.GotFocus += TbChat_GotFocus;
        }

        private void TbChat_GotFocus(object sender, RoutedEventArgs e)
        {
            tb_search_seller.SetText("Vendeur...");
            tb_search_card.SetText("Carte...");
            tb_search_max_price.SetText("Prix max...");
            tb__search_max_quantity.SetText("Quantité max...");
            ((TextBox)sender).Text = "";
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TbChat_MaxQuantity_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int research;
            if (int.TryParse(tb__search_max_quantity.GetText(), out research))
            {

                if (brocanteList.ItemsSource != null)
                    brocanteList.ItemsSource = null;
                else
                    brocanteList.Items.Clear();

                if (research.ToString() != string.Empty)
                {
                    foreach (BrocanteCard card in _cards)
                        if (card.Quantity <= research)
                            brocanteList.Items.Add(card);
                }
                else
                    brocanteList.ItemsSource = _cards;
            }
        }

        private void TbChat_MaxPrice_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int research;
            if (int.TryParse(tb_search_max_price.GetText(), out research))
            {

                if (brocanteList.ItemsSource != null)
                    brocanteList.ItemsSource = null;
                else
                    brocanteList.Items.Clear();

                if (research.ToString() != string.Empty)
                {
                    foreach (BrocanteCard card in _cards)
                        if (card.Price <= research)
                            brocanteList.Items.Add(card);
                }
                else
                    brocanteList.ItemsSource = _cards;
            }
        }

        private void TbChat_Seller_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string research = tb_search_seller.GetText().ToUpper();

            if (brocanteList.ItemsSource != null)
                brocanteList.ItemsSource = null;
            else
                brocanteList.Items.Clear();

            if (research != string.Empty)
            {
                foreach (BrocanteCard card in _cards)
                    if (card.SellerName.ToUpper().StartsWith(research))
                        brocanteList.Items.Add(card);
            }
            else
                brocanteList.ItemsSource = _cards;
        }

        private void TbChat_Card_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                string research = tb_search_card.GetText().ToUpper();

                if (brocanteList.ItemsSource != null)
                    brocanteList.ItemsSource = null;
                else
                    brocanteList.Items.Clear();

                if (research != string.Empty)
                {
                    foreach (BrocanteCard card in _cards)
                    {
                        logger.Trace(card.CardName);
                        if (card.CardName.ToUpper().StartsWith(research))
                            brocanteList.Items.Add(card);
                    }
                }
                else
                    brocanteList.ItemsSource = _cards;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        private void brocanteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BrocanteCard card = ((sender as ListView).SelectedItem as BrocanteCard);

                if (card == null)
                    return;

                DisplayCardInfo.SetCard(CardManager.GetCard(card.Id));
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _admin.CloseBrocante();
            _admin.LoadBrocante -= _admin_LoadBrocante;
        }

        private void btnSell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.AskSelectCard();
            SelectCard form = new SelectCard(_admin.Client.SelectCardAdmin);
            form.Owner = this;
            form.SelectedCard += Form_SelectedCard;
            form.Show();
        }

        private void Form_SelectedCard(PlayerCard card, int price, int quantity)
        {
            _admin.SellBrocanteCard(card, price, quantity);
        }

        private void btnBuy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BrocanteCard card = (brocanteList.SelectedItem as BrocanteCard);
            _admin.BuyBrocanteCard(card);
        }

        private void btnMyCards_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (searchMyCard)
            {
                brocanteList.ItemsSource = null;
                foreach (BrocanteCard card in _cards)
                    if (CheckIsOwn(card))
                        brocanteList.Items.Add(card);

                searchMyCard = !searchMyCard;
                btnMyCards.ButtonText = "Toutes cartes";
                btnMyCards.Update();
            }
            else
            {
                if (brocanteList.ItemsSource != null)
                    brocanteList.ItemsSource = null;
                brocanteList.Items.Clear();
                brocanteList.ItemsSource = _cards;

                searchMyCard = !searchMyCard;
                btnMyCards.ButtonText = "Mes cartes";
                btnMyCards.Update();
            }
        }
        private bool CheckIsOwn(BrocanteCard card)
        {
            return card.SellerName == FormExecution.Client.Username();
        }

        private void lvUsersColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                brocanteList.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            brocanteList.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(60, 0, 60, 100);
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

        public class SortAdorner : Adorner
        {
            private static Geometry ascGeometry =
                Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

            private static Geometry descGeometry =
                Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

            public ListSortDirection Direction { get; private set; }

            public SortAdorner(UIElement element, ListSortDirection dir)
                : base(element)
            {
                this.Direction = dir;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (AdornedElement.RenderSize.Width < 20)
                    return;

                TranslateTransform transform = new TranslateTransform
                    (
                        AdornedElement.RenderSize.Width - 15,
                        (AdornedElement.RenderSize.Height - 5) / 2
                    );
                drawingContext.PushTransform(transform);

                Geometry geometry = ascGeometry;
                if (this.Direction == ListSortDirection.Descending)
                    geometry = descGeometry;
                drawingContext.DrawGeometry(Brushes.Black, null, geometry);

                drawingContext.Pop();
            }
        }
    }
}
