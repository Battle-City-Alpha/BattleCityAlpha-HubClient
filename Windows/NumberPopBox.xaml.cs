using hub_client.Configuration;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour NumberPopBox.xaml
    /// </summary>
    public partial class NumberPopBox : Window
    {
        public event Action<int> SelectedNumber;

        int _price;
        public NumberPopBox(int maxNumber, int price)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            for (int i = 1; i <= maxNumber; i++)
                cbNumber.Items.Add(i);
            cbNumber.SelectionChanged += CbNumber_SelectionChanged;

            _price = price;
            cbNumber.SelectedIndex = 0;


            btnAgree.MouseLeftButtonDown += BtnAgree_MouseLeftButtonDown;

            this.MouseDown += Window_MouseDown;
        }

        private void CbNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lb_price.Content = Convert.ToInt32(cbNumber.SelectedItem) * _price;
        }

        private void BtnAgree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedNumber?.Invoke(Convert.ToInt32(cbNumber.SelectedItem));
            Close();
        }

        public void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;

            btnAgree.Color1 = style.GetGameColor("Color1PopBoxButton");
            btnAgree.Color2 = style.GetGameColor("Color2PopBoxButton");
            btnAgree.Update();

            this.FontFamily = style.Font;
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
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
