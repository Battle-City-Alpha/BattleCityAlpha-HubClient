using BCA.Common;
using BCA.Network.Packets.Enums;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
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
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour AnimationsSchedule.xaml
    /// </summary>
    public partial class AnimationsSchedule : Window
    {
        AnimationsScheduleAdministrator _admin;
        public AnimationsSchedule(AnimationsScheduleAdministrator admin, Animation[] animations, Dictionary<string, string> colors)
        {
            InitializeComponent();

            _admin = admin;

            this.MouseDown += Window_MouseDown;

            LoadColors(colors);
            LoadAnims(animations);

            if (FormExecution.PlayerInfos.Rank >= PlayerRank.Animateurs)
            {
                btn_create.Visibility = Visibility.Visible;
                btn_create.MouseLeftButtonDown += Btn_create_MouseLeftButtonDown;
            }
            else
                btn_create.Visibility = Visibility.Hidden;

            LoadStyle();
        }

        public void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_create });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = FormExecution.AppDesignConfig.GetGameColor("Color1AnimationPlanning");
                btn.Color2 = FormExecution.AppDesignConfig.GetGameColor("Color2AnimationPlanning");
                btn.Update();
            }

            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }

        private void Btn_create_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (FormExecution.PlayerInfos.Rank < PlayerRank.Animateurs)
                return;

            UpdateAnimation control = new UpdateAnimation(null);
            control.Owner = this;
            control.Show();
            control.SendUpdate += Control_SendUpdate;
        }

        private void Control_SendUpdate(Animation anim, bool remove)
        {
            _admin.SendAnimationUpdate(anim, remove);
        }

        private void LoadColors(Dictionary<string, string> colors)
        {
            foreach (var infos in colors)
            {
                Label lbl = new Label();
                lbl.Content = infos.Key;
                lbl.FontSize = 20;
                lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
                lbl.HorizontalAlignment = HorizontalAlignment.Center;
                lbl.VerticalAlignment = VerticalAlignment.Center;

                Rectangle rect = new Rectangle();
                rect.Width = 15;
                rect.Height = 15;
                rect.Margin = new Thickness(5, 0, 5, 0);
                rect.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(infos.Value));

                colors_panel.Children.Add(lbl);
                colors_panel.Children.Add(rect);
            }
        }
        public void LoadAnims(Animation[] anims)
        {
            List<int> toRemove = new List<int>();
            for (int i = 0; i < planning.Children.Count; i++)
                if (planning.Children[i] is BCA_Animation)
                    toRemove.Add(i);
            int offset = 0;
            foreach (int elem in toRemove)
            {
                planning.Children.RemoveAt(elem - offset);
                offset++;
            }

            foreach (Animation anim in anims)
            {
                int dayofweek = (int)anim.StartDate.DayOfWeek;
                if (dayofweek == 0)
                    dayofweek = 7;
                BCA_Animation widget = new BCA_Animation(anim, _admin);
                planning.Children.Add(widget);
                Grid.SetColumn(widget, dayofweek);
                Grid.SetRow(widget, anim.StartDate.Hour - 14);
                Grid.SetRowSpan(widget, anim.Duration);

                Storyboard storyboard = new Storyboard();

                ScaleTransform scale = new ScaleTransform(0.0, 0.0);
                widget.RenderTransformOrigin = new Point(0.5, 0.5);
                widget.RenderTransform = scale;

                DoubleAnimation growAnimationOpenX = new DoubleAnimation();
                growAnimationOpenX.Duration = TimeSpan.FromMilliseconds(200);
                growAnimationOpenX.From = 0.0;
                growAnimationOpenX.To = 1.0;
                DoubleAnimation growAnimationOpenY = new DoubleAnimation();
                growAnimationOpenY.Duration = TimeSpan.FromMilliseconds(200);
                growAnimationOpenY.From = 0.0;
                growAnimationOpenY.To = 1.0;
                storyboard.Children.Add(growAnimationOpenX);
                storyboard.Children.Add(growAnimationOpenY);
                Storyboard.SetTargetProperty(growAnimationOpenX, new PropertyPath("RenderTransform.ScaleX"));
                Storyboard.SetTargetProperty(growAnimationOpenY, new PropertyPath("RenderTransform.ScaleY"));
                Storyboard.SetTarget(growAnimationOpenX, widget);
                Storyboard.SetTarget(growAnimationOpenY, widget);
                storyboard.Begin();
            }
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(30);
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
