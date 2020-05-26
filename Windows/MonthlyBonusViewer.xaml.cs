using BCA.Common;
using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Stuff;
using hub_client.Windows.Controls;
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
    /// Logique d'interaction pour MonthlyBonusViewer.xaml
    /// </summary>
    public partial class MonthlyBonusViewer : Window
    {
        MonthlyBonus current_bonus;
        int cnumber;
        public MonthlyBonusViewer(Dictionary<int, MonthlyBonus> bonus, int connectionnumber, int[] cards)
        {
            InitializeComponent();
            cnumber = connectionnumber;

            foreach (var b in bonus)
            {
                BCA_MonthlyBonus widget = new BCA_MonthlyBonus(b.Value, b.Key, connectionnumber < b.Key,  connectionnumber == b.Key);
                wp_bonus.Children.Add(widget);
            }
            current_bonus = bonus[connectionnumber];
            if (current_bonus.Type == BonusType.Booster)
                current_bonus.Cards = cards;

            btn_get.MouseLeftButtonDown += Btn_get_MouseLeftButtonDown;

            this.Loaded += MonthlyBonusViewer_Loaded;
        }
        private void LoadStyle()
        {
            List<BCA_ColorButton> RankedButtons = new List<BCA_ColorButton>();
            RankedButtons.AddRange(new[] { btn_get });

            AppDesignConfig style = FormExecution.AppDesignConfig;

            foreach (BCA_ColorButton btn in RankedButtons)
            {
                btn.Color1 = style.GetGameColor("Color1MonthlyBonusViewer");
                btn.Color2 = style.GetGameColor("Color2MonthlyBonusViewer");
                btn.Update();
            }
            this.FontFamily = style.Font;
        }

        private void MonthlyBonusViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (cnumber > 8 && cnumber <= 16)
                scrv_bonus.ScrollToVerticalOffset((int)scrv_bonus.ScrollableHeight / 3);
            else if (cnumber > 16 && cnumber <= 24)
                scrv_bonus.ScrollToVerticalOffset((int)scrv_bonus.ScrollableHeight / 3 * 2);
            else if (cnumber > 24)
                scrv_bonus.ScrollToVerticalOffset((int)scrv_bonus.ScrollableHeight / 3 * 3);

            LoadStyle();
        }

        private void Btn_get_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (current_bonus.Type == BonusType.Booster) {
                FormExecution.OpenPurchase(new BoosterInfo { Name = current_bonus.Gift, Type = PurchaseType.Booster }, current_bonus.Cards);
            }
            else
            {
                BonusBox box = new BonusBox(current_bonus.Type, cnumber, current_bonus.Gift);
                box.Show();
                box.Topmost = true;
            }
            Close();
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
}
