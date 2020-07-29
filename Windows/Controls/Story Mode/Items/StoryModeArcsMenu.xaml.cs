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
    /// Logique d'interaction pour StoryModeArcsMenu.xaml
    /// </summary>
    public partial class StoryModeArcsMenu : UserControl
    {
        public StoryModeArcsMenu()
        {
            InitializeComponent();
        }
        public void InitItem(int id, string title, string infos)
        {
            ImageBrush background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/BCA_StoryMode;component/Assets/StoryMode/Menus/arcsmenu_" + id + ".jpg")));
            background.Stretch = Stretch.UniformToFill;
            bg_polygon.Fill = background;

            this.tb_title.Text = title;
            this.tb_infos.Text = infos;
        }

        public void ItemSelected()
        {
            bg_polygon.BeginAnimation(Path.StrokeThicknessProperty, new DoubleAnimation(bg_polygon.StrokeThickness, 10.0, TimeSpan.FromMilliseconds(200)));
            bd_title.BeginAnimation(HeightProperty, new DoubleAnimation(bd_title.Height, 60.0, TimeSpan.FromMilliseconds(200)));
            bg_polygon.Stroke = new SolidColorBrush(Colors.Black);
            bd_title.Background = new SolidColorBrush(Colors.Black);
            tb_title.Foreground = new SolidColorBrush(Colors.White);
            tb_infos.Foreground = new SolidColorBrush(Colors.White);
        }

        public void ItemUnselected()
        {
            bg_polygon.BeginAnimation(Path.StrokeThicknessProperty, new DoubleAnimation(bg_polygon.StrokeThickness, 5.0, TimeSpan.FromMilliseconds(200)));
            bd_title.BeginAnimation(HeightProperty, new DoubleAnimation(bd_title.Height, 30.0, TimeSpan.FromMilliseconds(200)));
            bg_polygon.Stroke = new SolidColorBrush(Colors.White);
            bd_title.Background = new SolidColorBrush(Colors.White);
            tb_title.Foreground = new SolidColorBrush(Colors.Black);
            tb_infos.Foreground = new SolidColorBrush(Colors.Black);
        }
    }
}
