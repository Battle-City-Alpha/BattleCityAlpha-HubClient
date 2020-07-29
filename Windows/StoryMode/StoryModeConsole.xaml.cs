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
            this.btn_bottom.MouseLeftButtonDown += (sender, e) => JoystickMove(sender, e, DirectionEnum.Down);
            this.btn_top.MouseLeftButtonDown += (sender, e) => JoystickMove(sender, e, DirectionEnum.Up);
            this.btn_top.PreviewMouseUp += (sender, e) => JoystickMove(sender, e, DirectionEnum.IDLELeft);
            this.btn_bottom.PreviewMouseUp += (sender, e) => JoystickMove(sender, e, DirectionEnum.IDLERight);

            this.KeyUp += PlayerKeyUp;
            this.KeyDown += PlayerKeyDown;

            this.home_border.MouseLeftButtonDown += (sender, e) => SkipTitleScreen();
            this.home_pic.MouseLeftButtonDown += (sender, e) => SkipTitleScreen();

            LoadStyle();

            LoadTitleScreen();
        }

        private void LoadStyle()
        {
            this.bg_border.Background = new SolidColorBrush(FormExecution.AppDesignConfig.GameColors["ConsoleBackground"]);
        }
        private void JoystickMove(object sender, MouseButtonEventArgs e, DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Left:
                    _joystick.Left();
                    ((Border)sender).BorderThickness = new Thickness(8);
                    break;
                case DirectionEnum.Right:
                    _joystick.Right();
                    ((Border)sender).BorderThickness = new Thickness(8);
                    break;
                case DirectionEnum.IDLELeft:
                    _joystick.IDLELeft();
                    ((Border)sender).BorderThickness = new Thickness(9);
                    break;
                case DirectionEnum.IDLERight:
                    _joystick.IDLERight();
                    ((Border)sender).BorderThickness = new Thickness(9);
                    break;
                case DirectionEnum.Up:
                    _joystick.Up();
                    ((Border)sender).BorderThickness = new Thickness(8);
                    break;
                case DirectionEnum.Down:
                    _joystick.Down();
                    ((Border)sender).BorderThickness = new Thickness(8);
                    break;
            }
            e.Handled = true;
        }

        private void LoadTitleScreen()
        {
            TitleScreen scene = new TitleScreen(_joystick);
            scene.SkipTitleScreen += SkipTitleScreen;
            bd_black_fade.BeginAnimation(OpacityProperty, new DoubleAnimation(bd_black_fade.Opacity, 0.0, TimeSpan.FromMilliseconds(200)));
            console_screen.Child = scene;
        }
        private void SkipTitleScreen()
        {
            _joystick.MustCloseEvent();
            MainMenu menu = new MainMenu(_joystick);
            menu.MakeChoice += MenuChoice;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(200);
            opacityAnimation.From = bd_black_fade.Opacity;
            opacityAnimation.To = 1.0;
            storyboard.Children.Add(opacityAnimation);

            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(opacityAnimation, bd_black_fade);

            storyboard.Completed += (sender, e) => EndStoryboardLoadMainMenu (sender, e, menu);
            storyboard.Begin();

        }
        private void EndStoryboardLoadMainMenu(object sender, EventArgs e, MainMenu menu)
        {
            bd_black_fade.BeginAnimation(OpacityProperty, new DoubleAnimation(bd_black_fade.Opacity, 0.0, TimeSpan.FromMilliseconds(200)));
            console_screen.Child = menu;
        }
        private void LoadOpenWorld()
        {
            OpenWorldScenes scenes = new OpenWorldScenes(new OpenWorldCharacter[] { new OpenWorldCharacter(100004, 1) }, _joystick);
            scenes.OpenDialog += OpenDialogScenes;
            console_screen.Child = scenes;
        }
        private void LoadArcsMenu()
        {
            ArcsMenu menu = new ArcsMenu(_joystick);
            console_screen.Child = menu;
        }

        private void OpenDialogScenes(Scene scene)
        {
            DialogScene dialog = new DialogScene(scene, _joystick);
            dialog.CloseScene += Dialog_CloseScene;
            bd_black_fade.BeginAnimation(OpacityProperty, new DoubleAnimation(bd_black_fade.Opacity, 0.0, TimeSpan.FromMilliseconds(200)));
            console_screen.Child = dialog;
        }
        private void Dialog_CloseScene()
        {
            console_screen.Child = null;
            bd_black_fade.BeginAnimation(OpacityProperty, new DoubleAnimation(bd_black_fade.Opacity, 1.0, TimeSpan.FromMilliseconds(200)));
        }

        private void MenuChoice(int choice)
        {
            switch (choice)
            {
                case 1:
                    LoadArcsMenu();
                    break;
                case 2:
                    LoadOpenWorld();
                    break;
                case 3:
                    StartClosing();
                    break;
            }
        }

        private void PlayerKeyUp(object sender, KeyEventArgs e)
        {
            _joystick.KeyReleased(sender, e);
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
            _joystick.KeyPress(sender, e);
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
            StartClosing();
        }
        private void StartClosing()
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