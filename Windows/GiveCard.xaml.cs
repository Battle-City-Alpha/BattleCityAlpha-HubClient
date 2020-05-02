﻿using BCA.Common;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Windows.Controls;
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
    /// Logique d'interaction pour GiveCard.xaml
    /// </summary>
    public partial class GiveCard : Window
    {
        private GiveCardAdministrator _admin;

        private PlayerInfo _target;
        private Dictionary<int, PlayerCard> _cards;

        public GiveCard(GiveCardAdministrator admin, PlayerInfo target)
        {
            InitializeComponent();
            LoadStyle();

            _cards = new Dictionary<int, PlayerCard>();
            _target = target;
            _admin = admin;

            _admin.LoadCards += _admin_LoadCards;

            btnAdd.MouseLeftButtonDown += BtnAdd_MouseLeftButtonDown;
            btnSelect.MouseLeftButtonDown += BtnSelect_MouseLeftButtonDown;

            Collection.GetListview().SelectionChanged += GiveCard_SelectionChanged;
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
            _admin.SendGiveCard(_cards, _target);
            Close();
        }

        private void BtnAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Collection.SelectedIndex() == -1) return;

            PlayerCard card = ((PlayerCard)Collection.SelectedItem());
            if (!Collection.RemoveCard(card))
                return;

            lb_choice.Items.Add(card.Name);

            PlayerCard offerCard = new PlayerCard();
            offerCard.Id = card.Id;
            offerCard.Name = card.Name;
            offerCard.Quantity = 1;

            if (!_cards.ContainsKey(offerCard.Id))
                _cards.Add(offerCard.Id, offerCard);
            else
                _cards[offerCard.Id].Quantity++;
        }

        private void _admin_LoadCards(Dictionary<int, PlayerCard> cards)
        {
            Collection.UpdateCollection(cards);
        }

        public void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btnAdd, btnSelect });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1BrocanteButton");
                btn.Color2 = style.GetGameColor("Color2BrocanteButton");
                btn.Update();
            }

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
