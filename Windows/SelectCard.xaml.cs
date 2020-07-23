using BCA.Common;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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

        private DispatcherTimer _popupTimer;

        public SelectCard(SelectCardAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _admin = admin;
            _admin.LoadSelectCard += _admin_LoadSelectCard;

            Collection.GetListview().SelectionChanged += SelectCard_SelectionChanged;

            _popupTimer = new DispatcherTimer();
            _popupTimer.Interval = TimeSpan.FromMilliseconds(1000);
            _popupTimer.Tick += _popupTimer_Tick;
            _popupTimer.IsEnabled = false;

            Closed += SelectCard_Closed;

            LoadStyle();

            this.MouseDown += Window_MouseDown;
        }
        private void _popupTimer_Tick(object sender, EventArgs e)
        {
            sell_card_popup.IsOpen = false;
            _popupTimer.IsEnabled = false;
        }

        private void SelectCard_Closed(object sender, EventArgs e)
        {
            _admin.LoadSelectCard -= _admin_LoadSelectCard;
            Collection.GetListview().SelectionChanged -= SelectCard_SelectionChanged;
        }

        private void _admin_LoadSelectCard(Dictionary<int, PlayerCard> cards, bool cardSold)
        {
            Collection.UpdateCollection(cards);
            if (this.IsActive)
            {
                this.sell_card_popup.IsOpen = true;
                _popupTimer.IsEnabled = true;
            }

            if (selected_index != -1 && Collection.GetListview().Items.Count > selected_index)
            {
                Collection.GetListview().SelectedIndex = selected_index;
            }
        }

        private void SelectCard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Collection.SelectedItem() == null) return;
                DisplayCardInfo.SetCard(CardManager.GetCard(((PlayerCard)Collection.SelectedItem()).Id));
                selected_index = Collection.SelectedIndex();
            }
            catch { };
        }

        private void BCA_ColorButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Collection.SelectedItem() == null || tb_price.Text == string.Empty || Convert.ToInt32(tb_price.Text) <= 0 || tb_quantity.Text == string.Empty || Convert.ToInt32(tb_quantity.Text) <= 0) return;
            selected_index = Collection.SelectedIndex();

            SelectedCard?.Invoke((PlayerCard)Collection.SelectedItem(), Convert.ToInt32(tb_price.Text), Convert.ToInt32(tb_quantity.Text));
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;

            btnSelect.Color1 = style.GetGameColor("Color1BrocanteButton");
            btnSelect.Color2 = style.GetGameColor("Color2BrocanteButton");
            btnSelect.Update();

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
