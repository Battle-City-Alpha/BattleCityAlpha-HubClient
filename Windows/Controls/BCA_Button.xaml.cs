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
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 1;
            da.To = 0.5;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            da.AutoReverse = true;
            da.RepeatBehavior = RepeatBehavior.Forever;
            border.Background.BeginAnimation(OpacityProperty, da);
            border.Background.Opacity = 0.5;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0.5;
            da.To = 1;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            da.AutoReverse = true;
            da.RepeatBehavior = RepeatBehavior.Forever;
            border.Background.BeginAnimation(OpacityProperty, da);
            border.Background.Opacity = 1;
        }
    }
}
