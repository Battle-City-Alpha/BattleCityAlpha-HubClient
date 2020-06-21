using BCA.Common;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour GiveCard.xaml
    /// </summary>
    public partial class GiveCard : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private GiveCardAdministrator _admin;

        private Dictionary<int, PlayerCard> _cards;
        private List<int> _ids;

        public event Action<Dictionary<int, PlayerCard>> SelectedCards;

        public GiveCard(GiveCardAdministrator admin, string target)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            LoadStyle();

            _cards = new Dictionary<int, PlayerCard>();
            _admin = admin;
            _ids = new List<int>();
            this.lbl_target.Content = target;

            _admin.LoadCards += _admin_LoadCards;

            btnAdd.MouseLeftButtonDown += BtnAdd_MouseLeftButtonDown;
            btnSelect.MouseLeftButtonDown += BtnSelect_MouseLeftButtonDown;

            Collection.GetListview().SelectionChanged += GiveCard_SelectionChanged;

            lb_choice.MouseDoubleClick += Lb_choice_MouseDoubleClick;
            lb_choice.SelectionChanged += Lb_choice_SelectionChanged; ;

            this.MouseDown += Window_MouseDown;
        }

        private void Lb_choice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_choice.SelectedIndex == -1) return;
            string item = lb_choice.SelectedItem.ToString();

            int id = _ids[lb_choice.SelectedIndex];
            if (!_cards.ContainsKey(id))
                return;
            try
            {
                DisplayCardInfo.SetCard(CardManager.GetCard(id));
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
            }
        }

        private void Lb_choice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lb_choice.SelectedIndex == -1) return;
            string item = lb_choice.SelectedItem.ToString();

            int id = _ids[lb_choice.SelectedIndex];

            if (!_cards.ContainsKey(id))
                return;
            else
            {
                if (_cards[id].Quantity > 1)
                    _cards[id].Quantity--;
                else
                    _cards.Remove(id);
            }

            Collection.AddCard(id);
            _ids.RemoveAt(lb_choice.SelectedIndex);
            lb_choice.Items.RemoveAt(lb_choice.SelectedIndex);
        }

        private void GiveCard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Collection.SelectedItem() == null) return;
                DisplayCardInfo.SetCard(CardManager.GetCard(((PlayerCard)Collection.SelectedItem()).Id));
            }
            catch { };
        }

        private void BtnSelect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedCards?.Invoke(_cards);
            Close();
        }

        private void BtnAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Collection.SelectedIndex() == -1) return;

            PlayerCard card = ((PlayerCard)Collection.SelectedItem());
            if (!Collection.RemoveCard(card))
                return;

            PlayerCard offerCard = new PlayerCard();
            offerCard.Id = card.Id;
            offerCard.Name = card.Name;
            offerCard.Quantity = 1;

            if (!_cards.ContainsKey(offerCard.Id))
                _cards.Add(offerCard.Id, offerCard);
            else
                _cards[offerCard.Id].Quantity++;

            _ids.Add(offerCard.Id);

            lb_choice.Items.Add(card.Name);
        }

        private void _admin_LoadCards(Dictionary<int, PlayerCard> cards)
        {
            Collection.UpdateCollection(cards);
        }

        public void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btnSelect });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1BrocanteButton");
                btn.Color2 = style.GetGameColor("Color2BrocanteButton");
                btn.Update();
            }

            btnAdd.Color1 = style.GetGameColor("Color1CenterBrocanteButton");
            btnAdd.Color2 = style.GetGameColor("Color2CenterBrocanteButton");
            btnAdd.Update();

            this.FontFamily = style.Font;
        }
        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.ActivateChat();
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(40, 0, 40, 40);
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
}
