using hub_client.Configuration;
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
            btnAgree.Color1 = style.Color1PopBoxButton;
            btnAgree.Color2 = style.Color2PopBoxButton;
            btnAgree.Update();
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
