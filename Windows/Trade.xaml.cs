using BCA.Common;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Trade.xaml
    /// </summary>
    public partial class Trade : Window
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        bool validate = false;
        bool endTrade = false;

        private TradeAdministrator _admin;

        private int _id;
        private PlayerInfo[] _players = new PlayerInfo[2];
        Dictionary<int, PlayerCard> _cardsToOffer = new Dictionary<int, PlayerCard>();

        int _idCardShow = -1;

        public Trade(TradeAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _admin = admin;

            Closed += Trade_Closed;
            this.MouseDown += Window_MouseDown;


            _admin.InitTrade += _admin_InitTrade;
            _admin.GetMessage += _admin_GetMessage;
            _admin.UpdateCardsToOffer += _admin_UpdateCardsToOffer;
            _admin.TradeExit += _admin_TradeExit;
            _admin.TradeEnd += _admin_TradeEnd;

            CollectionJ1.GetListview().SelectionChanged += lvPlayer1_SelectionChanged;
            CollectionJ2.GetListview().SelectionChanged += lvPlayer2_SelectionChanged;

            LoadStyle();

            img_card.MouseEnter += Img_card_MouseEnter;
            img_card.MouseLeave += Img_card_MouseLeave;
            img_card.PreviewMouseWheel += Img_card_PreviewMouseWheel;
        }

        private void Img_card_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scrollPos = scrCardDesc.VerticalOffset - e.Delta;
            if (scrollPos < 0)
                scrollPos = 0;
            if (scrollPos > scrCardDesc.ScrollableHeight)
                scrollPos = (int)scrCardDesc.ScrollableHeight;

            scrCardDesc.ScrollToVerticalOffset(scrollPos);
            e.Handled = true;
        }

        private void Img_card_MouseLeave(object sender, MouseEventArgs e)
        {
            this.cardinfos_popup.IsOpen = false;
        }

        private void Img_card_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                CardInfos info = CardManager.GetCard(_idCardShow);
                SetCard(info);
                this.cardinfos_popup.IsOpen = true;
            }
            catch (Exception ex)
            {
                logger.Error("POPUP PURCHASE - {0}", ex.ToString());
            }
        }

        private void SetCard(CardInfos card)
        {
            tb_cardname.Text = card.Name;
            CardType[] typeArray = card.GetCardTypes();
            string level = "";
            string atkdef = "";
            string attribute = "";
            if (typeArray.Contains(CardType.Magie) || typeArray.Contains(CardType.Piège))
            {

            }
            else
            {
                if (!typeArray.Contains(CardType.Link))
                {
                    if (typeArray.Contains(CardType.Pendule))
                        level = string.Format("◊{0}    {1}◊", card.LScale, card.RScale);
                    else
                        level = card.Level + "★";

                    atkdef = string.Format("{0}/{1}", card.Atk, card.Def);
                }
                else
                {
                    LinkMarker[] markers = card.GetLinkMarkers();
                    atkdef = card.Atk + "/LINK-" + markers.Count();

                    level = GetStringLinksMarkers(markers);
                }
                attribute = string.Format("{0}|{1}", card.GetRace(), card.GetAttribute());
            }
            tb_cardlevel.Text = level;
            tb_cardatkdef.Text = atkdef;
            tb_cardattribute.Text = attribute;
            tb_cardtype.Text = card.GetCardType();
            tb_carddesc.Text = card.Description;

        }
        private string GetStringLinksMarkers(IEnumerable<LinkMarker> types)
        {
            string toReturn = "";
            foreach (var linkmarker in types)
            {
                switch (linkmarker)
                {
                    case LinkMarker.BottomLeft:
                        toReturn += "[↙]";
                        break;
                    case LinkMarker.Bottom:
                        toReturn += "[↓]";
                        break;
                    case LinkMarker.BottomRight:
                        toReturn += "[↘]";
                        break;
                    case LinkMarker.Left:
                        toReturn += "[←]";
                        break;
                    case LinkMarker.Right:
                        toReturn += "[→]";
                        break;
                    case LinkMarker.TopLeft:
                        toReturn += "[↖]";
                        break;
                    case LinkMarker.Top:
                        toReturn += "[↑]";
                        break;
                    case LinkMarker.TopRight:
                        toReturn += "[↗]";
                        break;
                }
            }
            return toReturn;
        }

        private void _admin_TradeEnd()
        {
            endTrade = true;
            Close();
        }
        private void _admin_TradeExit()
        {
            endTrade = true;
            Close();
        }
        private void Trade_Closed(object sender, EventArgs e)
        {
            if (!endTrade)
                _admin.SendTradeExit(_id);

            Closed -= Trade_Closed;


            _admin.InitTrade -= _admin_InitTrade;
            _admin.GetMessage -= _admin_GetMessage;
            _admin.UpdateCardsToOffer -= _admin_UpdateCardsToOffer;
            _admin.TradeExit -= _admin_TradeExit;
            _admin.TradeEnd -= _admin_TradeEnd;

            CollectionJ1.GetListview().SelectionChanged -= lvPlayer1_SelectionChanged;
            CollectionJ2.GetListview().SelectionChanged -= lvPlayer2_SelectionChanged;
        }

        private void _admin_UpdateCardsToOffer(List<PlayerCard> cards)
        {
            CollectionJ1.Clear();
            foreach (var card in _cardsToOffer)
                CollectionJ1.Add(card.Value);
            CollectionJ2.Clear();
            foreach (PlayerCard card in cards)
                CollectionJ2.Add(card);

            btnValidate.IsEnabled = true;
        }

        private void _admin_GetMessage(PlayerInfo user, string message)
        {
            Dispatcher.InvokeAsync(delegate { chat.OnPlayerColoredMessage(FormExecution.AppDesignConfig.GetGameColor("StandardMessageColor"), user, message, true); });
        }

        private void _admin_InitTrade(int id, PlayerInfo[] players, Dictionary<int, BCA.Common.PlayerCard>[] Collections)
        {
            _id = id;
            _players = players;
            this.Title = _players[0].Username + " & " + _players[1].Username;
            this.nameJ1.Text = "Collection de " + _players[0].Username;
            this.nameJ2.Text = "Collection de " + _players[1].Username;

            CollectionJ1.UpdateCollection(Collections[0]);
            CollectionJ2.UpdateCollection(Collections[1]);

            btnValidate.IsEnabled = true;
        }

        private void lvPlayer1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CollectionJ1.SelectedItem() == null) return;
            _idCardShow = ((PlayerCard)CollectionJ1.SelectedItem()).Id;
            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", ((PlayerCard)CollectionJ1.SelectedItem()).Id + ".jpg" });
        }
        private void lvPlayer2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CollectionJ2.SelectedItem() == null) return;
            _idCardShow = ((PlayerCard)CollectionJ2.SelectedItem()).Id;
            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", ((PlayerCard)CollectionJ2.SelectedItem()).Id + ".jpg" });
        }

        private void BCA_ColorButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CollectionJ1.SelectedIndex() == -1) return;

            PlayerCard card = ((PlayerCard)CollectionJ1.SelectedItem());
            if (!CollectionJ1.RemoveCard(card))
                return;

            lb_choice.Items.Add(card.Name + "(" + card.Id + ")");

            PlayerCard offerCard = new PlayerCard();
            offerCard.Id = card.Id;
            offerCard.Name = card.Name;
            offerCard.Quantity = 1;

            if (!_cardsToOffer.ContainsKey(offerCard.Id))
                _cardsToOffer.Add(offerCard.Id, offerCard);
            else
                _cardsToOffer[offerCard.Id].Quantity++;
        }

        private void tbChat_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    _admin.SendTradeMessage(_id, tbChat.GetText());
                    tbChat.Clear();
                    break;
            }
            e.Handled = true;
        }

        private void lb_choice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string item = lb_choice.SelectedItem.ToString();
            if (lb_choice.SelectedIndex == -1) return;

            string id = item.Split('(')[1].Replace(")", string.Empty);

            if (!_cardsToOffer.ContainsKey(Convert.ToInt32(id)))
                return;
            else
            {
                if (_cardsToOffer[Convert.ToInt32(id)].Quantity > 1)
                    _cardsToOffer[Convert.ToInt32(id)].Quantity--;
                else
                    _cardsToOffer.Remove(Convert.ToInt32(id));
            }



            PlayerCard card = CollectionJ1.AddCard(Convert.ToInt32(id));
            lb_choice.Items.RemoveAt(lb_choice.SelectedIndex);
        }

        private void BCA_ColorButton_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (lb_choice.Items.Count <= 0)
            {
                FormExecution.Client_PopMessageBox("Il faut proposer au moins une carte !", "Echange", true);
                return;
            }
            if (!validate)
            {
                btnProposition.IsEnabled = false;
                btnValidate.IsEnabled = false;
                validate = true;
                _admin.SendTradeProposition(_id, GlobalTools.GetDictionnaryValues(_cardsToOffer));
            }
            else
            {
                btnProposition.IsEnabled = false;
                btnValidate.IsEnabled = false;
                _admin.SendTradeAnswer(_id, true);
            }
        }

        public void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;

            btnProposition.Color1 = style.GetGameColor("Color1TradeButton");
            btnProposition.Color2 = style.GetGameColor("Color2TradeButton");
            btnProposition.Update();

            btnValidate.Color1 = style.GetGameColor("Color1TradeButton");
            btnValidate.Color2 = style.GetGameColor("Color2TradeButton");
            btnValidate.Update();

            this.FontFamily = style.Font;

            this.chat.chat.VerticalScrollBarVisibility = FormExecution.ClientConfig.TradeScrollBar ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;

            if (FormExecution.ClientConfig.TradeScrollBar)
            {
                this.CollectionJ1.GetListview().SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
                this.CollectionJ2.GetListview().SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            }
            else
            {
                this.CollectionJ1.GetListview().SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);
                this.CollectionJ2.GetListview().SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);
            }
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
