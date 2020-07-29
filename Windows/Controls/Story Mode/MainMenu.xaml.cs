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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hub_client.Windows.Controls.Story_Mode
{
    /// <summary>
    /// Logique d'interaction pour MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public event Action<int> MakeChoice;

        private int _itemselect = 1;
        private Joystick _joystick;

        public MainMenu(Joystick joystick)
        {
            InitializeComponent();

            _joystick = joystick;
            _joystick.KeyUp += _joystick_KeyUp;
            _joystick.PressUp += () => ChangeItem(true);
            _joystick.PressDown += () => ChangeItem(false);
            _joystick.CloseEvents += CloseEvents;

            LoadItem();
            SelectItem();
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
                    MakeChoice?.Invoke(_itemselect);
                    CloseEvents();
                    break;
            }
        }

        private void ChangeItem(bool up)
        {
            int offset = up ? -1 : 1;
            if (_itemselect + offset < 1 || _itemselect + offset > 3)
                return;

            UnselectItem();
            _itemselect += offset;
            SelectItem();
        }

        private void LoadItem()
        {
            item_adventure.InitItem(1, "Aventure");
            item_free.InitItem(3, "Monde libre");
            item_quit.InitItem(4, "Quitter");
        }

        public void SelectItem()
        {
            switch (_itemselect)
            {
                case 1:
                    bd_firstitem.BeginAnimation(WidthProperty, new DoubleAnimation(bd_firstitem.Width, 340, TimeSpan.FromMilliseconds(200)));
                    item_adventure.ItemSelected();
                    break;
                case 2:
                    bd_second.BeginAnimation(WidthProperty, new DoubleAnimation(bd_second.Width, 340, TimeSpan.FromMilliseconds(200)));
                    item_free.ItemSelected();
                    break;
                case 3:
                    bd_third.BeginAnimation(WidthProperty, new DoubleAnimation(bd_third.Width, 340, TimeSpan.FromMilliseconds(200)));
                    item_quit.ItemSelected();
                    break;
            }
        }
        public void UnselectItem()
        {
            switch (_itemselect)
            {
                case 1:
                    bd_firstitem.BeginAnimation(WidthProperty, new DoubleAnimation(bd_firstitem.Width, 0, TimeSpan.FromMilliseconds(200)));
                    item_adventure.ItemUnselected();
                    break;
                case 2:
                    bd_second.BeginAnimation(WidthProperty, new DoubleAnimation(bd_second.Width, 0, TimeSpan.FromMilliseconds(200)));
                    item_free.ItemUnselected();
                    break;
                case 3:
                    bd_third.BeginAnimation(WidthProperty, new DoubleAnimation(bd_third.Width, 0, TimeSpan.FromMilliseconds(200)));
                    item_quit.ItemUnselected();
                    break;
            }
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
