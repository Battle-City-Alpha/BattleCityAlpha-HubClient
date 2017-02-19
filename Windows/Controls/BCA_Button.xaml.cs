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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_Button.xaml
    /// </summary>
    public partial class BCA_Button : UserControl
    {
        public BCA_Button()
        {
            InitializeComponent();
            text.InitLabel("Jouer", Colors.White, (Color)ColorConverter.ConvertFromString("#FF0600FF"), 22);
        }

        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            border.BorderThickness = new Thickness(3);
        }

        private void border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            border.BorderThickness = new Thickness(2);
        }
    }
}
