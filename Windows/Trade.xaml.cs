using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Cards;
using hub_client.Network;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logique d'interaction pour Trade.xaml
    /// </summary>
    public partial class Trade : Window
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        int selected_index = -1;

        bool validate = false;

        private TradeAdministrator _admin;

        private int _id;
        private PlayerInfo[] _players = new PlayerInfo[2];
        Dictionary<int, BCA.Common.PlayerCard>[] _collections;
        List<PlayerCard> _cardsToOffer = new List<PlayerCard>();

        public Trade(TradeAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;

            this.Closed += Trade_Closed;


            _admin.InitTrade += _admin_InitTrade;
            _admin.GetMessage += _admin_GetMessage;
            _admin.UpdateCardsToOffer += _admin_UpdateCardsToOffer;
            _admin.TradeExit += _admin_TradeExit;

            tb_search1.tbChat.TextChanged += TbChat_TextChanged1;
            tb_search2.tbChat.TextChanged += TbChat_TextChanged2;
        }

        private void _admin_TradeExit()
        {
            Close();
        }

        private void Trade_Closed(object sender, EventArgs e)
        {
            _admin.Client.Send(PacketType.TradeExit, new StandardClientTradeExit { Id = _id });
        }

        private void _admin_UpdateCardsToOffer(List<PlayerCard> cards)
        {
            lvPlayer1.Items.Clear();
            foreach (PlayerCard card in _cardsToOffer)
                lvPlayer1.Items.Add(card);
            lvPlayer2.Items.Clear();
            foreach (PlayerCard card in cards)
                lvPlayer2.Items.Add(card);

            btnValidate.IsEnabled = true;
        }

        private void TbChat_TextChanged2(object sender, TextChangedEventArgs e)
        {
            lvPlayer2.Items.Clear();
            foreach (var var in _collections[1])
                if (!string.IsNullOrEmpty(tb_search2.GetText()) && var.Value.Name.ToLower().Contains(tb_search2.GetText().ToLower()))
                    lvPlayer2.Items.Add(var.Value);
            if (tb_search2.GetText() == "")
            {
                foreach (var args in _collections[1])
                {
                    args.Value.Name = CardManager.GetCard(args.Key).Name;
                    lvPlayer2.Items.Add(args.Value);
                }
            }
        }

        private void TbChat_TextChanged1(object sender, TextChangedEventArgs e)
        {
            lvPlayer1.Items.Clear();
            foreach (var var in _collections[0])
                if (!string.IsNullOrEmpty(tb_search1.GetText()) && var.Value.Name.ToLower().Contains(tb_search1.GetText().ToLower()))
                    lvPlayer1.Items.Add(var.Value);
            if (tb_search1.GetText() == "")
            {
                foreach (var args in _collections[0])
                {
                    args.Value.Name = CardManager.GetCard(args.Key).Name;
                    lvPlayer1.Items.Add(args.Value);
                }
            }
        }

        private void _admin_GetMessage(string username, string message)
        {
            Dispatcher.InvokeAsync(delegate { chat.OnColoredMessage(FormExecution.AppDesignConfig.StandardMessageColor, "["+username+"]:"+message, false, false); });
        }

        private void _admin_InitTrade(int id, PlayerInfo[] players, Dictionary<int, BCA.Common.PlayerCard>[] Collections)
        {
            _id = id;
            _players = players;
            this.Title = _players[0].Username + " & " + _players[1].Username;
            _collections = Collections;
            UpdateCollection();
        }

        private void UpdateCollection()
        {
            lvPlayer1.Items.Clear();
            lvPlayer2.Items.Clear();

            foreach (var args in _collections[0])
            {
                args.Value.Name = CardManager.GetCard(args.Key).Name;
                lvPlayer1.Items.Add(args.Value);
            }

            foreach (var args in _collections[1])
            {
                args.Value.Name = CardManager.GetCard(args.Key).Name;
                lvPlayer2.Items.Add(args.Value);
            }
        }

        private void ChangeCollection(PlayerCard card, bool add, ListView lv)
        { 
            int index = lv.Items.IndexOf(card);
            if (index != -1)
            {
                lv.Items.RemoveAt(index);
                lv.Items.Insert(index, card);
            }
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvPlayer1.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lvPlayer1.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void GridViewColumnHeader2_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvPlayer2.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lvPlayer2.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
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

        private void lvPlayer1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvPlayer1.SelectedItem == null) return;
            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", ((PlayerCard)lvPlayer1.SelectedItem).Id + ".jpg" });
            selected_index = lvPlayer1.SelectedIndex;
        }

        private void lvPlayer2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvPlayer2.SelectedItem == null) return;
            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", ((PlayerCard)lvPlayer2.SelectedItem).Id + ".jpg" });
        }

        private void BCA_ColorButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (lvPlayer1.SelectedIndex == -1) return;

            PlayerCard card = ((PlayerCard)lvPlayer1.SelectedItem);
            if (_collections[0][card.Id].Quantity == 0)
                return;

            _collections[0][card.Id].Quantity--;
            ChangeCollection(card, false, lvPlayer1);


            lb_choice.Items.Add(card.Name + "("+card.Id+")");
            _cardsToOffer.Add(card);
            lvPlayer1.SelectedIndex = selected_index;
        }

        private void tbChat_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    _admin.Client.Send(PacketType.TradeMessage, new StandardClientTradeMessage { Id = _id, Message = tbChat.GetText() });
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
            _collections[0][Convert.ToInt32(id)].Quantity++;
            lb_choice.Items.RemoveAt(lb_choice.SelectedIndex);
            ChangeCollection(_collections[0][Convert.ToInt32(id)], true, lvPlayer1);
            lvPlayer1.SelectedIndex = selected_index;
            _cardsToOffer.Remove(_collections[0][Convert.ToInt32(id)]);
        }

        private void BCA_ColorButton_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (lb_choice.Items.Count <= 0)
                return;
            if (!validate)
            {
                btnProposition.IsEnabled = false;
                btnValidate.IsEnabled = false;
                validate = true;
                _admin.Client.Send(PacketType.TradeProposition, new StandardClientTradeProposition { Id = _id, Cards = _cardsToOffer });
            }
        }
    }
}
