using BCA.Common;
using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.Helpers;
using hub_client.Windows.Controls;
using NLog;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour MonthlyBonusViewer.xaml
    /// </summary>
    public partial class MonthlyBonusViewer : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        MonthlyBonus current_bonus;
        int cnumber;
        public MonthlyBonusViewer(Dictionary<int, MonthlyBonus> bonus, int connectionnumber, int[] cards)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            cnumber = connectionnumber;

            logger.Trace("{0} | {1} | {2}", bonus, connectionnumber, cards);

            foreach (var b in bonus)
            {
                logger.Trace("{0} {1}", b.Key, b.Value.Gift);
                BCA_MonthlyBonus widget = new BCA_MonthlyBonus(b.Value, b.Key, connectionnumber < b.Key, connectionnumber == b.Key);
                wp_bonus.Children.Add(widget);
            }
            current_bonus = bonus[connectionnumber];
            if (current_bonus.Type == BonusType.Booster)
                current_bonus.Cards = cards;

            btn_get.MouseLeftButtonDown += Btn_get_MouseLeftButtonDown;

            this.Loaded += MonthlyBonusViewer_Loaded;

            this.MouseDown += Window_MouseDown;
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
            if (current_bonus.Type == BonusType.Booster)
            {
                FormExecution.OpenPurchase(BoosterManager.GetBoosterInfo(current_bonus.Gift), current_bonus.Cards);
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
