using hub_client.Configuration;
using System;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour PopBox.xaml
    /// </summary>
    public partial class PopBox : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        public PopBox(string txt, string title)
        {
            InitializeComponent();
            popText.Text = txt;

            Loaded += PopBox_Loaded;

            Title = title;
        }

        public void LoadStyle()
        {
            btnAgree.Color1 = style.GetGameColor("Color1PopBoxButton");
            btnAgree.Color2 = style.GetGameColor("Color2PopBoxButton");
            btnAgree.Update();

            this.FontFamily = style.Font;
        }

        private void PopBox_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
        }

        private void btnAgree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Loaded -= PopBox_Loaded;
        }
    }
}
