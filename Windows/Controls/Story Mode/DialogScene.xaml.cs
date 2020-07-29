using BCA_StoryMode.Models;
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
    /// Logique d'interaction pour DialogScene.xaml
    /// </summary>
    public partial class DialogScene : UserControl
    {
        public event Action CloseScene;

        private Scene _scene;
        private int _state;
        private int _textDisplayIndex;
        private Sentence _currentSentence;
        private bool _lastSpeakerLeft;

        private DispatcherTimer _textTimer;

        private Joystick _joystick;

        public DialogScene(Scene scene, Joystick joystick)
        {
            InitializeComponent(); 
            _scene = scene;
            _joystick = joystick;

            LoadScene();
            StartDialog();
        }

        private void LoadScene()
        {
            ImageBrush background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/BCA_StoryMode;component/Assets/StoryMode/Backgrounds/" + _scene.Background + ".jpg")));
            bg_border.Background = background;
        }
        private void StartDialog()
        {
            _state = 0;
            _joystick.PressSpace += _joystick_PressSpace;
            _joystick.CloseEvents += CloseEvents;
            LoadDialogState();
        }

        private void _joystick_PressSpace()
        {
            if (_textTimer.IsEnabled)
            {
                tb_text.Text = _currentSentence.Text;
                _textTimer.Stop();
                _textDisplayIndex = 0;
            }
            else
            {
                _state++;
                LoadDialogState();
            }
        }

        private void LoadDialogState()
        {
            Sentence s = _scene.Dialog.GetSentence(_state);
            if (s == null)
            {
                CloseScene?.Invoke();
                return;
            }
            _currentSentence = s;
            ImageBrush character = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/BCA_StoryMode;component/Assets/StoryMode/Characters/Faces/" + _currentSentence.Sprite + ".png")));

            if (_state == 0)
            {
                character_pic_left.Source = character.ImageSource;
                if (_scene.ID > 100000)
                    Grid.SetColumnSpan(character_pic_left, 3);
                _lastSpeakerLeft = true;
            }
            else
            {
                if (_lastSpeakerLeft)
                {
                    Grid.SetZIndex(character_pic_left, -2);
                    Grid.SetZIndex(character_pic_right, -1);
                    Grid.SetColumn(character_pic_right, 1);
                    Grid.SetColumnSpan(character_pic_right, 2);
                    Grid.SetColumnSpan(character_pic_left, 1);
                    character_pic_right.Source = character.ImageSource;
                }
                else
                {
                    Grid.SetZIndex(character_pic_left, -1);
                    Grid.SetZIndex(character_pic_right, -2);
                    Grid.SetColumn(character_pic_right, 2);
                    Grid.SetColumnSpan(character_pic_left, 2);
                    Grid.SetColumnSpan(character_pic_right, 1);
                    character_pic_left.Source = character.ImageSource;
                }
                _lastSpeakerLeft = !_lastSpeakerLeft;
            }

            tb_character.Text = s.Speaker.Name;

            _textTimer = new DispatcherTimer();
            _textTimer.Interval = TimeSpan.FromMilliseconds(40);
            _textTimer.Start();
            _textTimer.Tick += _textTimer_Tick;
        }

        private void _textTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => DisplayAnimation());
        }

        private void DisplayAnimation()
        {
            _textDisplayIndex++;
            if (_textDisplayIndex > _currentSentence.Text.Length)
            {
                _textTimer.Stop();
                _textDisplayIndex = 0;
            }
            else
                Dispatcher.Invoke(() => tb_text.Text = _currentSentence.Text.Substring(0, _textDisplayIndex));
        }

        public void CloseEvents()
        {
            _joystick.PressSpace -= _joystick_PressSpace;
            _joystick.CloseEvents -= CloseEvents;
        }
    }
}