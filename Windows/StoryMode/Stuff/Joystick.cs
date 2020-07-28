using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Windows.StoryMode.Stuff
{
    public class Joystick
    {
        public event Action MoveLeft;
        public event Action MoveRight;
        public event Action StopMoveLeft;
        public event Action StopMoveRight;
        public event Action PressSpace;

        public void Right() { MoveRight?.Invoke(); }
        public void Left() { MoveLeft?.Invoke(); }
        public void IDLERight() { StopMoveRight?.Invoke(); }
        public void IDLELeft() { StopMoveLeft?.Invoke(); }

        public void Space() { PressSpace?.Invoke(); }

    }
}
