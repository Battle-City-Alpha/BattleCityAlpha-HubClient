using hub_client.Configuration;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_PurchaseShopItem.xaml
    /// </summary>
    public partial class BCA_PurchaseShopItem : UserControl
    {
        AppDesignConfig style = FormExecution.AppDesignConfig;

        DoubleAnimation fadeInBorder;
        DoubleAnimation fadeOutBorder;
        DoubleAnimation fadeInText;
        DoubleAnimation fadeOutText;

        public BCA_PurchaseShopItem()
        {
            InitializeComponent();
            LoadStyle();

            this.MouseEnter += BCA_PurchaseShopItem_MouseEnter;
            this.MouseLeave += BCA_PurchaseShopItem_MouseLeave;

            fadeInBorder = new DoubleAnimation();
            fadeInBorder.From = 0.3;
            fadeInBorder.To = 1;
            fadeInBorder.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            fadeOutBorder = new DoubleAnimation();
            fadeOutBorder.From = 1;
            fadeOutBorder.To = 0.3;
            fadeOutBorder.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            fadeInText = new DoubleAnimation();
            fadeInText.From = 0;
            fadeInText.To = 1;
            fadeInText.Duration = new Duration(TimeSpan.FromSeconds(0.7));

            fadeOutText = new DoubleAnimation();
            fadeOutText.From = 1;
            fadeOutText.To = 0;
            fadeOutText.Duration = new Duration(TimeSpan.FromSeconds(0.7));
        }

        private void BCA_PurchaseShopItem_MouseLeave(object sender, MouseEventArgs e)
        {
            tb_desc.BeginAnimation(OpacityProperty, fadeOutText);
            border_img.BeginAnimation(OpacityProperty, fadeInBorder);
        }

        private void BCA_PurchaseShopItem_MouseEnter(object sender, MouseEventArgs e)
        {
            tb_desc.BeginAnimation(OpacityProperty, fadeInText);
            border_img.BeginAnimation(OpacityProperty, fadeOutBorder);
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_purchase });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1PrestigeShopButton");
                btn.Color2 = style.GetGameColor("Color2PrestigeShopButton");
                btn.Update();
            }

            this.FontFamily = style.Font;
        }

        public void RefreshStyle()
        {
            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }


        public void Initialize(string item, string price, string img_source, string desc)
        {
            tb_item.Text = item;
            tb_price.Text = price;
            border_img.Background = new ImageBrush(new BitmapImage(new Uri("Assets/Shop/" + img_source + ".png", UriKind.Relative)));
            tb_desc.Text = desc;
        }
    }
}
