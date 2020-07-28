using BCA_StoryMode.Models;
using BCA_StoryMode.OpenWorld;
using hub_client.Windows.Controls.Story_Mode;
using hub_client.Windows.StoryMode.Enums;
using hub_client.Windows.StoryMode.Stuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hub_client.Windows.StoryMode
{
    /// <summary>
    /// Logique d'interaction pour StoryModeConsole.xaml
    /// </summary>
    public partial class StoryModeConsole : Window
    {
        private Joystick _joystick;

        public StoryModeConsole()
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _joystick = new Joystick();
            this.btn_left.MouseDown += (sender, e) => JoystickMove(sender, e, DirectionEnum.Left);
            this.btn_right.MouseDown += (sender, e) => JoystickMove(sender, e, DirectionEnum.Right);
            this.btn_left.PreviewMouseUp += (sender, e) => JoystickMove(sender, e, DirectionEnum.IDLELeft);
            this.btn_right.PreviewMouseUp += (sender, e) => JoystickMove(sender, e, DirectionEnum.IDLERight);

            this.KeyUp += PlayerKeyUp;
            this.KeyDown += PlayerKeyDown;

            LoadOpenWorld();
        }

        private void JoystickMove(object sender, MouseButtonEventArgs e, DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Left:
                    _joystick.Left();
                    break;
                case DirectionEnum.Right:
                    _joystick.Right();
                    break;
                case DirectionEnum.IDLELeft:
                    _joystick.IDLELeft();
                    break;
                case DirectionEnum.IDLERight:
                    _joystick.IDLERight();
                    break;
            }
            e.Handled = true;
        }

        private void LoadOpenWorld()
        {
            OpenWorldScenes scenes = new OpenWorldScenes(new OpenWorldCharacter[] { new OpenWorldCharacter(100004, 1) }, _joystick);
            scenes.OpenDialog += OpenDialogScenes;

            console_screen.Child = scenes;
        }

        private void OpenDialogScenes(Scene scene)
        {
            DialogScene dialog = new DialogScene(scene, _joystick);
            dialog.CloseScene += Dialog_CloseScene;
            console_screen.Child = dialog;
        }

        private void Dialog_CloseScene()
        {
            console_screen.Child = null;
            bd_black_fade.BeginAnimation(OpacityProperty, new DoubleAnimation(bd_black_fade.Opacity, 1.0, TimeSpan.FromMilliseconds(200)));
        }

        private void PlayerKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    _joystick.IDLERight();
                    break;
                case Key.Left:
                    _joystick.IDLELeft();
                    break;
            }
        }
        private void PlayerKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    _joystick.Right();
                    break;
                case Key.Left:
                    _joystick.Left();
                    break;
                case Key.Space:
                    _joystick.Space();
                    break;
            }
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bd_power.Background = new SolidColorBrush(Colors.Red);
            Storyboard storyboard = new Storyboard();

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(200);
            opacityAnimation.From = bd_black_fade.Opacity;
            opacityAnimation.To = 1.0;
            storyboard.Children.Add(opacityAnimation);

            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(opacityAnimation, bd_black_fade);

            storyboard.Completed += CloseConsole;
            storyboard.Begin();
        }
        private void CloseConsole(object sender, EventArgs e)
        {
            Thread.Sleep(200);
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(50, 50, 0, 0);
            }
            else if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.bg_border.CornerRadius = new CornerRadius(0);
            }
        }
        private void minimizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }
    }
}