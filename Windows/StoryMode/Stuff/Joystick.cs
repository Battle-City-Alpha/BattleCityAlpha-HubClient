using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace hub_client.Windows.StoryMode.Stuff
{
    public class Joystick
    {
        public event Action MoveLeft;
        public event Action MoveRight;
        public event Action StopMoveLeft;
        public event Action StopMoveRight;
        public event Action PressSpace;
        public event Action PressUp;
        public event Action PressDown;
        public event Action CloseEvents;

        public event Action<object, KeyEventArgs> KeyUp;
        public event Action<object, KeyEventArgs> KeyDown;

        public void Right() { MoveRight?.Invoke(); }
        public void Left() { MoveLeft?.Invoke(); }
        public void IDLERight() { StopMoveRight?.Invoke(); }
        public void IDLELeft() { StopMoveLeft?.Invoke(); }

        public void Up() { PressUp?.Invoke(); }
        public void Down() { PressDown?.Invoke(); }

        public void Space() { PressSpace?.Invoke(); }

        public void KeyPress(object sender, KeyEventArgs e) { KeyUp?.Invoke(sender, e); }
        public void KeyReleased(object sender, KeyEventArgs e) { KeyDown?.Invoke(sender, e); }

        public void MustCloseEvent() { CloseEvents?.Invoke(); }

    }
}
