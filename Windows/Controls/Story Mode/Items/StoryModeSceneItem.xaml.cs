using BCA.Story_Mode;
using BCA.Story_Mode.Enums;
using BCA_StoryMode.Helpers;
using BCA_StoryMode.Models;
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
    /// Logique d'interaction pour StoryModeSceneItem.xaml
    /// </summary>
    public partial class StoryModeSceneItem : UserControl
    {
        SceneInfo _info;
        public StoryModeSceneItem(SceneInfo scene)
        {
            InitializeComponent();

            _info = scene;

            LoadScene();
        }

        private void LoadScene()
        {            
            switch (_info.State)
            {
                case SceneState.Unlock:
                    tb_infos.Text = "✅";
                    break;
                case SceneState.Lock:
                    tb_infos.Text = "🔒";
                    break;
            }
            Scene sc = ScenesManager.GetScene(_info.ID);
            if (sc == null)
                return;

            ImageBrush background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/BCA_StoryMode;component/Assets/StoryMode/Backgrounds/" + sc.Background + ".jpg")));
            background.Stretch = Stretch.UniformToFill;
            bg_polygon.Fill = background;

            this.tb_scene_name.Text = sc.Name.ToUpper();
        }

        public void ItemSelected()
        {
            bd_title.BeginAnimation(WidthProperty, new DoubleAnimation(bd_title.Width, 225, TimeSpan.FromMilliseconds(200)));
        }

        public void ItemUnselected()
        {
            bd_title.BeginAnimation(WidthProperty, new DoubleAnimation(bd_title.Width, 30.0, TimeSpan.FromMilliseconds(200))); 
        }
    }
}
