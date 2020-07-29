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

namespace hub_client.Windows.Controls.Story_Mode.Items
{
    /// <summary>
    /// Logique d'interaction pour StoryModeMenu.xaml
    /// </summary>
    public partial class StoryModeMenu : UserControl
    {
        public StoryModeMenu()
        {
            InitializeComponent();
        }

        public void InitItem(int id, string title)
        {
            ImageBrush background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/BCA_StoryMode;component/Assets/StoryMode/Menus/mainmenu_" + id + ".jpg")));
            background.Stretch = Stretch.UniformToFill;
            bg_hexagon.Fill = background;

            this.tb_title.Text = title;
        }

        public void ItemSelected()
        {
            bg_hexagon.BeginAnimation(Path.StrokeThicknessProperty, new DoubleAnimation(bg_hexagon.StrokeThickness, 5.0, TimeSpan.FromMilliseconds(200)));
            bg_hexagon.Stroke = new SolidColorBrush(Colors.Black);
            bd_title.Background = new SolidColorBrush(Colors.Black);
            tb_title.Foreground = new SolidColorBrush(Colors.White);
        }

        public void ItemUnselected()
        {
            bg_hexagon.BeginAnimation(Path.StrokeThicknessProperty, new DoubleAnimation(bg_hexagon.StrokeThickness, 1.0, TimeSpan.FromMilliseconds(200)));
            bg_hexagon.Stroke = new SolidColorBrush(Colors.White);
            bd_title.Background = new SolidColorBrush(Colors.White);
            tb_title.Foreground = new SolidColorBrush(Colors.Black);
        }
    }
}
