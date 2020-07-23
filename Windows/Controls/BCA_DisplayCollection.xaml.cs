using BCA.Common;
using hub_client.Cards;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_DisplayCollection.xaml
    /// </summary>
    public partial class BCA_DisplayCollection : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        Dictionary<int, PlayerCard> _collection;

        public BCA_DisplayCollection()
        {
            InitializeComponent();

            tb_search.tbChat.TextChanged += TbChat_TextChanged;
            tb_search.tbChat.IsEnabled = false;
        }

        public PlayerCard AddCard(int id)
        {
            if (!_collection.ContainsKey(id))
                return null;

            PlayerCard card = _collection[id];
            _collection[id].Quantity++;

            int index = lv.Items.IndexOf(card);
            if (index != -1)
            {
                lv.Items.RemoveAt(index);
                lv.Items.Insert(index, card);
            }
            lv.SelectedIndex = index;
            return card;
        }
        public bool RemoveCard(PlayerCard card)
        {
            if (_collection[card.Id].Quantity == 0)
                return false;

            _collection[card.Id].Quantity--;
            int index = lv.Items.IndexOf(card);
            if (index != -1)
            {
                lv.Items.RemoveAt(index);
                lv.Items.Insert(index, card);
                lv.Items.Refresh();
            }
            lv.SelectedIndex = index;
            return true;
        }
        public void UpdateCollection(Dictionary<int, PlayerCard> collection)
        {
            lv.Items.Clear();

            _collection = collection;
            foreach (var args in _collection)
            {
                CardInfos c = CardManager.GetCard(args.Key);
                if (c == null)
                {
                    args.Value.Name = "Id inconnue : " + args.Key;
                    continue;
                }
                args.Value.Name = c.Name;
                Add(args.Value);
            }

            tb_search.tbChat.IsEnabled = true;

            if (listViewSortCol != null)
            {
                SortColumn(listViewSortCol);
                SortColumn(listViewSortCol);
            }
        }

        private void TbChat_TextChanged(object sender, TextChangedEventArgs e)
        {
            Clear();
            foreach (var var in _collection)
                if (!string.IsNullOrEmpty(tb_search.GetText()) && var.Value.Name != null && var.Value.Name.ToLower().Contains(tb_search.GetText().ToLower()))
                    Add(var.Value);
            if (tb_search.GetText() == "")
            {
                foreach (var args in _collection)
                {
                    CardInfos c = CardManager.GetCard(args.Key);
                    if (c == null)
                    {
                        args.Value.Name = "Id inconnue : " + args.Key;
                        continue;
                    }
                    args.Value.Name = c.Name;
                    Add(args.Value);
                }
            }
        }

        public void Clear()
        {
            lv.Items.Clear();
        }
        public void Add(PlayerCard card)
        {
            lv.Items.Add(card);
        }
        public object SelectedItem()
        {
            return lv.SelectedItem;
        }
        public int SelectedIndex()
        {
            return lv.SelectedIndex;
        }
        public void SetIndex(int index)
        {
            lv.SelectedIndex = index;
        }
        public ListView GetListview()
        {
            return lv;
        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            SortColumn(column);

        }
        private void SortColumn(GridViewColumnHeader column)
        {
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lv.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lv.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
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
