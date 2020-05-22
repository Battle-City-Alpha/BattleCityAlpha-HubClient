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
    /// Logique d'interaction pour BCA_Smiley.xaml
    /// </summary>
    public partial class BCA_Smiley : UserControl
    {
        public event Action Clicked;

        public BCA_Smiley(Image smiley)
        {
            InitializeComponent();
            img_smiley.Source = smiley.Source.Clone();
            img_smiley.Width = FormExecution.AppDesignConfig.FontSize + 15;
            img_smiley.Height = FormExecution.AppDesignConfig.FontSize + 15;
            img_smiley.Margin = new Thickness(3);

            img_smiley.MouseEnter += Img_smiley_MouseEnter;
            img_smiley.MouseLeave += Img_smiley_MouseLeave;

            img_smiley.MouseLeftButtonDown += Img_smiley_MouseLeftButtonDown;
        }

        private void Img_smiley_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clicked?.Invoke();
        }

        private void Img_smiley_MouseLeave(object sender, MouseEventArgs e)
        {
            bg_smiley.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void Img_smiley_MouseEnter(object sender, MouseEventArgs e)
        {
            bg_smiley.Background = new SolidColorBrush(Colors.CornflowerBlue);
        }
    }
}
