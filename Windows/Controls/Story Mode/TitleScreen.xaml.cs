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
using System.Windows.Threading;

namespace hub_client.Windows.Controls.Story_Mode
{
    /// <summary>
    /// Logique d'interaction pour TitleScreen.xaml
    /// </summary>
    public partial class TitleScreen : UserControl
    {
        public event Action SkipTitleScreen;

        private Joystick _joystick;
        DispatcherTimer timer;

        public TitleScreen(Joystick joystick)
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            _joystick = joystick;
            _joystick.KeyUp += Joystick_KeyUp;
            _joystick.CloseEvents += CloseEvents;
        }

        private void CloseEvents()
        {
            _joystick.KeyUp -= Joystick_KeyUp;
            _joystick.CloseEvents -= CloseEvents;
        }

        private void Joystick_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    SkipTitleScreen?.Invoke();
                    CloseEvents();
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tb_start.Visibility = tb_start.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
