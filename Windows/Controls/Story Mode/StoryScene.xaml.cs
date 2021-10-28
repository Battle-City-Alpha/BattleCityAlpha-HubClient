using BCA_StoryMode.ARCS._5DS.Scenes.ScYusei;
using BCA_StoryMode.Helpers;
using BCA_StoryMode.Models;
using hub_client.Windows.StoryMode.Enums;
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
    /// Logique d'interaction pour StoryScene.xaml
    /// </summary>
    public partial class StoryScene : UserControl
    {
        public event Action CloseScene;
        public event Action<Scene> OpenDialog;

        private Scene _scene;

        private DispatcherTimer _moveTimer;
        private DirectionEnum[] _direction;
        private int[] _moveSpriteIndex;
        private TranslateTransform[] _moveTransform;
        private Image[] _charactersPics;

        private CharacterSprites[] _sprites;

        public StoryScene(Scene scene)
        {
            InitializeComponent(); 
            _scene = scene;
            _direction = new DirectionEnum[2];
            _moveSpriteIndex = new int[2];
            _moveTransform = new TranslateTransform[2];
            _sprites = new CharacterSprites[2];
            _charactersPics = new Image[] { character_pic_left, character_pic_right };

            LoadScene();
            LoadCharacters();

            _moveTimer = new DispatcherTimer();
            _moveTimer.Interval = TimeSpan.FromMilliseconds(40);
            _moveTimer.Tick += _moveTimer_Tick;
            _moveTimer.Start();
        }

        private void LoadScene()
        {
            ImageBrush background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/BCA_StoryMode;component/Assets/StoryMode/Backgrounds/" + _scene.Background + ".jpg")));
            background.Stretch = Stretch.UniformToFill;
            bg_border.Background = background;
        }
        private void LoadCharacters()
        {
            for (int i = 0; i < 2; i++)
            {
                _sprites[i] = SpritesHelper.CreateSprite(_scene.Characters[i]);
                _charactersPics[i].Height = _sprites[i].SpriteHeight;
                _charactersPics[i].Width = _sprites[i].SpriteWidth;

                _moveTransform[i] = new TranslateTransform(0, 0);
                _charactersPics[i].RenderTransform = _moveTransform[i];
            }


            _direction[0] = DirectionEnum.IDLERight;
            _direction[1] = DirectionEnum.IDLELeft;

            character_pic_left.Source = _sprites[0].IDLERight[0].ImageSource;
            character_pic_right.Source = _sprites[1].IDLELeft[0].ImageSource;

            this.KeyDown += PlayerKeyDown;
            this.KeyUp += PlayerKeyUp;
        }

        private void PlayerKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    _direction[0] = DirectionEnum.IDLERight;
                    break;
                case Key.Left:
                    _direction[0] = DirectionEnum.IDLELeft;
                    break;
            }
        }
        private void PlayerKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    _direction[0] = DirectionEnum.Right;
                    break;
                case Key.Left:
                    _direction[0] = DirectionEnum.Left;
                    break;
                case Key.Space:
                    TryOpenDialog();
                    break;
            }
        }

        private void _moveTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                _moveSpriteIndex[i]++;
                ImageBrush pic = null;
                switch (_direction[i])
                {
                    case DirectionEnum.Left:
                        if (_moveSpriteIndex[i] >= 6)
                            _moveSpriteIndex[i] = 0;
                        pic = _sprites[i].Left[_moveSpriteIndex[i]];

                        if (_moveTransform[i].X - 10 < 0)
                            _moveTransform[i].X = 0;
                        else
                            _moveTransform[i].X -= 10;
                        break;
                    case DirectionEnum.Right:
                        if (_moveSpriteIndex[i] >= 6)
                            _moveSpriteIndex[i] = 0;
                        pic = _sprites[i].Right[_moveSpriteIndex[i]];

                        if (_moveTransform[i].X + 10 >= this.Width - 140)
                            _moveTransform[i].X = (int)this.Width - 140;
                        else
                            _moveTransform[i].X += 10;
                        break;
                    case DirectionEnum.IDLERight:
                        if (_moveSpriteIndex[i] >= 10)
                            _moveSpriteIndex[i] = 0;
                        pic = _sprites[i].IDLERight[_moveSpriteIndex[i]];
                        break;
                    case DirectionEnum.IDLELeft:
                        if (_moveSpriteIndex[i] >= 10)
                            _moveSpriteIndex[i] = 0;
                        pic = _sprites[i].IDLELeft[_moveSpriteIndex[i]];
                        break;
                }

                _charactersPics[i].Source = pic.ImageSource;
            }
        }

        private void TryOpenDialog()
        {
            if (_moveTransform[0].X >= this.Width - 150)
            {
                OpenDialog?.Invoke(_scene);   
            }
        }
    }
}
