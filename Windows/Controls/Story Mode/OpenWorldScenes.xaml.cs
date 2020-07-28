using BCA_StoryMode.Helpers;
using BCA_StoryMode.Models;
using BCA_StoryMode.OpenWorld;
using hub_client.Windows.StoryMode.Enums;
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
using System.Windows.Threading;

namespace hub_client.Windows.Controls.Story_Mode
{
    /// <summary>
    /// Logique d'interaction pour OpenWorldScenes.xaml
    /// </summary>
    public partial class OpenWorldScenes : UserControl
    {
        public event Action CloseScene;
        public event Action<Scene> OpenDialog;

        private int _sceneIndex;
        private Scene _scene;

        private DispatcherTimer _moveTimer;

        private CharacterBody _mainCharacter;
        private List<CharacterBody> _currentCharacters;

        private bool _isMapChanging;

        private OpenWorldCharacter[] _characters;

        private Joystick _joystick;

        public OpenWorldScenes(OpenWorldCharacter[] characters, Joystick joystick)
        {
            InitializeComponent();

            _sceneIndex = 1;
            _isMapChanging = false;
            _characters = characters;

            _joystick = joystick;
            _joystick.MoveLeft += JoystickLeft;
            _joystick.MoveRight += JoystickRight;
            _joystick.StopMoveLeft += JoystickLeftReleased;
            _joystick.StopMoveRight += JoystickRightReleased;
            _joystick.PressSpace += CheckInteraction;

            LoadMainCharacter();
            LoadScene();

            _moveTimer = new DispatcherTimer();
            _moveTimer.Interval = TimeSpan.FromMilliseconds(40);
            _moveTimer.Tick += _moveTimer_Tick;
            _moveTimer.Start();
        }

        public void JoystickRightReleased()
        {
            _mainCharacter.Direction = DirectionEnum.IDLERight;
        }
        public void JoystickLeftReleased()
        {
            _mainCharacter.Direction = DirectionEnum.IDLELeft;
        }
        public void JoystickRight()
        {
            _mainCharacter.Direction = DirectionEnum.Right;
        }
        public void JoystickLeft()
        {
            _mainCharacter.Direction = DirectionEnum.Left;
        }

        private void LoadScene()
        {
            _scene = OpenWorldManager.GetScene(_sceneIndex);
            if (_currentCharacters != null)
                foreach (CharacterBody body in _currentCharacters)
                    if (body != _mainCharacter)
                        main_grid.Children.Remove(body.CharacterPic);

            _currentCharacters = new List<CharacterBody>();
            _currentCharacters.Add(_mainCharacter);

            ImageBrush background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/BCA_StoryMode;component/Assets/StoryMode/Backgrounds/" + _scene.Background + ".jpg")));
            main_grid.Background = background;

            tb_scene_name.Text = _scene.Name;
            dialog_pic.Visibility = Visibility.Hidden;

            LoadSceneCharacters();
        }
        private void LoadSceneCharacters()
        {
            foreach (OpenWorldCharacter character in _characters)
            {
                if (character.Scene == _scene.ID)
                {
                    Character player = CharactersManager.GetCharacter(character.Character);
                    if (player == null)
                        break;
                    CharacterBody newCharacter = new CharacterBody(player);
                    newCharacter.Sprites = SpritesHelper.CreateSprite(player);
                    newCharacter.CharacterPic = new Image();
                    newCharacter.CharacterPic.Stretch = Stretch.Uniform;
                    newCharacter.CharacterPic.Margin = new Thickness(0, 5, 0, 5);
                    newCharacter.CharacterPic.HorizontalAlignment = HorizontalAlignment.Left;
                    newCharacter.CharacterPic.VerticalAlignment = VerticalAlignment.Center;
                    newCharacter.CharacterDialogPic = dialog_pic;
                    main_grid.Children.Add(newCharacter.CharacterPic);
                    Grid.SetRow(newCharacter.CharacterPic, 2);
                    Grid.SetZIndex(newCharacter.CharacterPic, 2);

                    newCharacter.MoveTransform = new TranslateTransform(new Random().Next(30, (int)this.ActualWidth - 2 * newCharacter.Sprites.SpriteWidth), 0);
                    newCharacter.CharacterPic.RenderTransform = newCharacter.MoveTransform;
                    newCharacter.Direction = new Random().Next(2) == 0 ? DirectionEnum.IDLELeft : DirectionEnum.IDLERight;
                    newCharacter.MoveSpriteIndex = 0;

                    _currentCharacters.Add(newCharacter);
                }
            }
        }
        private void LoadMainCharacter()
        {
            Character player = CharactersManager.GetCharacter(2);
            if (player == null)
                CloseScene?.Invoke();

            _mainCharacter = new CharacterBody(player);
            _mainCharacter.CharacterPic = character_pic_left;
            _mainCharacter.Sprites = SpritesHelper.CreateSprite(player);
            character_pic_left.Height = _mainCharacter.Sprites.SpriteHeight;
            character_pic_left.Width = _mainCharacter.Sprites.SpriteWidth;
            _mainCharacter.MoveTransform = new TranslateTransform(0, 0);
            _mainCharacter.Direction = DirectionEnum.IDLERight;
            _mainCharacter.MoveSpriteIndex = 0;
            character_pic_left.RenderTransform = _mainCharacter.MoveTransform;

            character_pic_left.Source = _mainCharacter.Sprites.IDLERight[0].ImageSource;

            this.KeyDown += PlayerKeyDown;
            this.KeyUp += PlayerKeyUp;


            dialog_pic.Height = 30;
            dialog_pic.Width = _mainCharacter.Sprites.SpriteWidth;
        }

        private void PlayerKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    _mainCharacter.Direction = DirectionEnum.IDLERight;
                    break;
                case Key.Left:
                    _mainCharacter.Direction = DirectionEnum.IDLELeft;
                    break;
            }
        }
        private void PlayerKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    _mainCharacter.Direction = DirectionEnum.Right;
                    break;
                case Key.Left:
                    _mainCharacter.Direction = DirectionEnum.Left;
                    break;
                case Key.Space:
                    CheckInteraction();
                    break;
            }
        }

        private void _moveTimer_Tick(object sender, EventArgs e)
        {
            foreach (CharacterBody character in _currentCharacters)
            {
                character.MoveSpriteIndex++;
                ImageBrush pic = null;
                switch (character.Direction)
                {
                    case DirectionEnum.Left:
                        if (character.MoveSpriteIndex >= 6)
                            character.MoveSpriteIndex = 0;
                        pic = character.Sprites.Left[character.MoveSpriteIndex];

                        if (character.MoveTransform.X - 10 < 0)
                            ChangeSceneLeft();
                        else
                            character.MoveTransform.X -= 10;
                        break;
                    case DirectionEnum.Right:
                        if (character.MoveSpriteIndex >= 6)
                            character.MoveSpriteIndex = 0;
                        pic = character.Sprites.Right[character.MoveSpriteIndex];

                        if (character.MoveTransform.X + 10 >= main_grid.ActualWidth - character.Sprites.SpriteWidth)
                            ChangeSceneRight();
                        else
                            character.MoveTransform.X += 10;
                        break;
                    case DirectionEnum.IDLERight:
                        if (character.MoveSpriteIndex >= 10)
                            character.MoveSpriteIndex = 0;
                        pic = character.Sprites.IDLERight[character.MoveSpriteIndex];
                        break;
                    case DirectionEnum.IDLELeft:
                        if (character.MoveSpriteIndex >= 10)
                            character.MoveSpriteIndex = 0;
                        pic = character.Sprites.IDLELeft[character.MoveSpriteIndex];
                        break;
                }

                character.CharacterPic.Source = pic.ImageSource;

                if (character == _mainCharacter)
                    continue;
                character.HasCollisionWith(_mainCharacter);
            }
        }

        private void ChangeSceneRight()
        {
            if (OpenWorldManager.GetScene(_sceneIndex + 1) == null || _isMapChanging)
            {
                if (_mainCharacter.MoveTransform.X + 10 < main_grid.ActualWidth + _mainCharacter.Sprites.SpriteWidth)
                    _mainCharacter.MoveTransform.X += 10;
                else
                    _mainCharacter.MoveTransform.X = main_grid.ActualWidth + _mainCharacter.Sprites.SpriteWidth;
                return;
            }

            _isMapChanging = true;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(200);
            opacityAnimation.From = 0.0;
            opacityAnimation.To = 1.0;
            DoubleAnimation opacityAnimationCharacter = opacityAnimation.Clone();
            opacityAnimationCharacter.From = 1.0;
            opacityAnimationCharacter.To = 0.0;
            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(opacityAnimationCharacter);

            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(opacityAnimationCharacter, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(opacityAnimation, bd_black_fade);
            Storyboard.SetTarget(opacityAnimationCharacter, _mainCharacter.CharacterPic);

            storyboard.Completed += (sender, e) => FadeOut(sender, e, false);
            storyboard.Begin();
        }
        private void ChangeSceneLeft()
        {
            if (OpenWorldManager.GetScene(_sceneIndex - 1) == null || _isMapChanging)
            {
                if (_mainCharacter.MoveTransform.X - 10 > 0 - _mainCharacter.Sprites.SpriteWidth)
                    _mainCharacter.MoveTransform.X -= 10;
                else
                    _mainCharacter.MoveTransform.X = 0 - _mainCharacter.Sprites.SpriteWidth;
                return;
            }

            _isMapChanging = true;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(200);
            opacityAnimation.From = 0.0;
            opacityAnimation.To = 1.0;
            DoubleAnimation opacityAnimationCharacter = opacityAnimation.Clone();
            opacityAnimationCharacter.From = 1.0;
            opacityAnimationCharacter.To = 0.0;
            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(opacityAnimationCharacter);

            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(opacityAnimationCharacter, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(opacityAnimation, bd_black_fade);
            Storyboard.SetTarget(opacityAnimationCharacter, _mainCharacter.CharacterPic);

            storyboard.Completed += (sender, e) => FadeOut(sender, e, true);
            storyboard.Begin();
        }
        private void FadeOut(object sender, EventArgs e, bool left)
        {
            if (left)
            {
                _mainCharacter.MoveTransform.X = main_grid.ActualWidth - _mainCharacter.Sprites.SpriteWidth - 10;
                _mainCharacter.Direction = DirectionEnum.IDLELeft;
                _sceneIndex--;
            }
            else
            {
                _mainCharacter.MoveTransform.X = 10;
                _mainCharacter.Direction = DirectionEnum.IDLERight;
                _sceneIndex++;
            }
            LoadScene();

            Storyboard storyboard = new Storyboard();

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(200);
            opacityAnimation.From = 1.0;
            opacityAnimation.To = 0.0;
            DoubleAnimation opacityAnimationCharacter = opacityAnimation.Clone();
            opacityAnimationCharacter.From = 0.0;
            opacityAnimationCharacter.To = 1.0;
            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(opacityAnimationCharacter);

            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(opacityAnimationCharacter, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(opacityAnimation, bd_black_fade);
            Storyboard.SetTarget(opacityAnimationCharacter, _mainCharacter.CharacterPic);

            storyboard.Completed += FadeIn;
            storyboard.Begin();
        }
        private void FadeIn(object sender, EventArgs e)
        {
            _isMapChanging = false;
        }

        private void CheckInteraction()
        {
            foreach (CharacterBody rigidbody in _currentCharacters)
            {
                if (rigidbody == _mainCharacter)
                    continue;
                if (!rigidbody.HasCollisionWith(_mainCharacter))
                    continue;

                Scene dialogScene = ScenesManager.GetScene(_scene.ID);
                dialogScene.Dialog = rigidbody.SpawnDialog;
                OpenDialog?.Invoke(dialogScene);
                CloseEvent();
            }
        }

        private void CloseEvent()
        {
            _joystick.MoveLeft -= JoystickLeft;
            _joystick.MoveRight -= JoystickRight;
            _joystick.StopMoveLeft -= JoystickLeftReleased;
            _joystick.StopMoveRight -= JoystickRightReleased;
            _joystick.PressSpace -= CheckInteraction;
        }
    }
}