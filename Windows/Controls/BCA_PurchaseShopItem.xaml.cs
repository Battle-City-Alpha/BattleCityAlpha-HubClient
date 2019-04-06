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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_PurchaseShopItem.xaml
    /// </summary>
    public partial class BCA_PurchaseShopItem : UserControl
    {
        public static readonly DependencyProperty ItemTextProperty = DependencyProperty.Register
              (
                   "ItemText",
                   typeof(string),
                   typeof(BCA_PurchaseShopItem),
                   new PropertyMetadata(string.Empty)
              );

        public string ItemText
        {
            get { return (string)GetValue(ItemTextProperty); }
            set { SetValue(ItemTextProperty, value); }
        }

        public static readonly DependencyProperty PriceTextProperty = DependencyProperty.Register
              (
                   "PriceText",
                   typeof(string),
                   typeof(BCA_PurchaseShopItem),
                   new PropertyMetadata(string.Empty)
              );

        public string PriceText
        {
            get { return (string)GetValue(PriceTextProperty); }
            set { SetValue(PriceTextProperty, value); }
        }

        public BCA_PurchaseShopItem()
        {
            InitializeComponent();
        }

        public void Initialize(string item, string price, string img_source)
        {
            tb_item.Text = item;
            tb_price.Text = price;
            border_img.Background = new ImageBrush(new BitmapImage(new Uri("Assets/Shop/"+img_source+".png", UriKind.Relative)));
        }
    }
}
