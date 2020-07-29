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
    /// Logique d'interaction pour ArcsMenu.xaml
    /// </summary>
    public partial class ArcsMenu : UserControl
    {
        public event Action<int> MakeChoice;

        private int _itemselect = 1;
        private Joystick _joystick;

        public ArcsMenu(Joystick joystick)
        {
            InitializeComponent();
            _joystick = joystick;
            _joystick.KeyUp += _joystick_KeyUp;
            _joystick.MoveLeft += () => ChangeItem(true);
            _joystick.MoveRight += () => ChangeItem(false);
            _joystick.CloseEvents += CloseEvents;

            LoadItem();
            SelectItem();
        }

        private void _joystick_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    ChangeItem(true);
                    break;
                case Key.Right:
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
            if (_itemselect + offset < 1 || _itemselect + offset > 2)
                return;

            UnselectItem();
            _itemselect += offset;
            SelectItem();
        }

        private void LoadItem()
        {
            item_5DS.InitItem(1, "ARC 5DS", "4 SCENARIOS");
            item_GX.InitItem(2, "ARC GX", "A VENIR...");
        }

        public void SelectItem()
        {
            switch (_itemselect)
            {
                case 1:
                    item_5DS.ItemSelected();
                    break;
                case 2:
                    item_GX.ItemSelected();
                    break;
            }
        }
        public void UnselectItem()
        {
            switch (_itemselect)
            {
                case 1:
                    item_5DS.ItemUnselected();
                    break;
                case 2:
                    item_GX.ItemUnselected();
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