using BCA.Story_Mode;
using hub_client.Windows.Controls.Story_Mode.Items;
using hub_client.Windows.StoryMode.Stuff;
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

namespace hub_client.Windows.Controls.Story_Mode
{
    /// <summary>
    /// Logique d'interaction pour ChooseSceneMenu.xaml
    /// </summary>
    public partial class ChooseSceneMenu : UserControl
    {
        public event Action<SceneInfo> MakeChoice;

        private SceneInfo[] _scenes;
        private Joystick _joystick;
        private int _itemselect;

        public ChooseSceneMenu(SceneInfo[] infos, Joystick joystick)
        {
            InitializeComponent();

            _scenes = infos; _joystick = joystick;
            _joystick.KeyUp += _joystick_KeyUp;
            _joystick.MoveLeft += () => ChangeItem(true);
            _joystick.MoveRight += () => ChangeItem(false);
            _joystick.CloseEvents += CloseEvents;

            LoadItems();
            SelectItem();
        }

        private void LoadItems()
        {
            foreach (SceneInfo info in _scenes)
            {
                StoryModeSceneItem item = new StoryModeSceneItem(info);
                panel_scenes.Children.Add(item);
            }
        }

        private void _joystick_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    ChangeItem(true);
                    break;
                case Key.Down:
                    ChangeItem(false);
                    break;
                case Key.Enter:
                case Key.Space:
                    MakeChoice?.Invoke(_scenes[_itemselect]);
                    CloseEvents();
                    break;
            }
        }

        private void ChangeItem(bool up)
        {
            int offset = up ? -1 : 1;
            if (_itemselect + offset < 0 || _itemselect + offset >= _scenes.Length)
                return;

            UnselectItem();
            _itemselect += offset;
            SelectItem();
        }

        public void SelectItem()
        {
            ((StoryModeSceneItem)panel_scenes.Children[_itemselect]).ItemSelected();
        }
        public void UnselectItem()
        {
            ((StoryModeSceneItem)panel_scenes.Children[_itemselect]).ItemUnselected();
        }

        public void CloseEvents()
        {
            _joystick.PressUp -= () => ChangeItem(true);
            _joystick.PressDown -= () => ChangeItem(false);
            _joystick.KeyUp -= _joystick_KeyUp;
            _joystick.CloseEvents -= CloseEvents;
        }
    }
}
