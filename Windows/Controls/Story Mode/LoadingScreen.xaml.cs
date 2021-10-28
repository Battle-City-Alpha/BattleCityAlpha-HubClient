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
    /// Logique d'interaction pour LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : UserControl
    {
        DispatcherTimer timer;
        DispatcherTimer timertxt;

        private int _lastBg;
        private int _pts;

        public LoadingScreen()
        {
            InitializeComponent();

            _lastBg = 1;
            _pts = 0;
            
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(2000);
            timer.Tick += Timer_Tick;
            timer.Start();

            timertxt = new DispatcherTimer();
            timertxt.Interval = TimeSpan.FromMilliseconds(500);
            timertxt.Tick += Timertxt_Tick; ;
            timertxt.Start();
        }

        private void Timertxt_Tick(object sender, EventArgs e)
        {
            _pts++;
            if (_pts > 3)
                _pts = 0;

            string txt = "CHARGEMENT";
            for (int i = 0; i < _pts; i++)
                txt += ".";

            tb_loading.Text = txt;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int newbg = new Random().Next(1, 7);
            while (_lastBg == newbg) 
                newbg = new Random().Next(1, 7);
            _lastBg = newbg;

            Storyboard storyboard = new Storyboard();

            RotateTransform scale = new RotateTransform(0.0);
            hexagon_loading.RenderTransformOrigin = new Point(0.5, 0.5);
            hexagon_loading.RenderTransform = scale;

            DoubleAnimation anim = new DoubleAnimation();
            anim.Duration = TimeSpan.FromMilliseconds(300);
            anim.From = hexagon_loading.StrokeThickness;
            anim.To = 120.0;
            storyboard.Children.Add(anim);
            DoubleAnimation rotateanim = new DoubleAnimation();
            rotateanim.Duration = TimeSpan.FromMilliseconds(300);
            rotateanim.From = 0.0;
            rotateanim.To = 180.0;
            storyboard.Children.Add(anim);
            storyboard.Children.Add(rotateanim);
            Storyboard.SetTargetProperty(rotateanim, new PropertyPath("RenderTransform.Angle"));
            Storyboard.SetTargetProperty(anim, new PropertyPath(Path.StrokeThicknessProperty));
            Storyboard.SetTarget(anim, hexagon_loading);
            Storyboard.SetTarget(rotateanim, hexagon_loading);

            storyboard.Completed += ChangeBackground;

            storyboard.Begin();
        }

        private void ChangeBackground(object sender, EventArgs e)
        {
            ImageBrush background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/BCA_StoryMode;component/Assets/StoryMode/Loading/" + _lastBg + ".png")));
            background.Stretch = Stretch.UniformToFill;
            hexagon_loading.Fill = background;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation anim = new DoubleAnimation();
            anim.Duration = TimeSpan.FromMilliseconds(300);
            anim.From = hexagon_loading.StrokeThickness;
            anim.To = 5.0;
            DoubleAnimation rotateanim = new DoubleAnimation();
            rotateanim.Duration = TimeSpan.FromMilliseconds(300);
            rotateanim.From = 180.0;
            rotateanim.To = 360.0;

            storyboard.Children.Add(anim);
            storyboard.Children.Add(rotateanim);

            Storyboard.SetTargetProperty(rotateanim, new PropertyPath("RenderTransform.Angle"));
            Storyboard.SetTargetProperty(anim, new PropertyPath(Path.StrokeThicknessProperty));
            Storyboard.SetTarget(anim, hexagon_loading);
            Storyboard.SetTarget(rotateanim, hexagon_loading);

            storyboard.Begin();
        }
    }
}
