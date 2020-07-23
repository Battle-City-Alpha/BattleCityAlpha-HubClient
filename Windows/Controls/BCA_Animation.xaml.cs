using BCA.Common;
using BCA.Network.Packets.Enums;
using hub_client.WindowsAdministrator;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_Animation.xaml
    /// </summary>
    public partial class BCA_Animation : UserControl
    {
        AnimationsScheduleAdministrator _admin;
        Animation _anim;
        DispatcherTimer _timer;

        public BCA_Animation(Animation anim, AnimationsScheduleAdministrator admin)
        {
            InitializeComponent();

            _anim = anim;
            _admin = admin;

            this.tb_popup_anim_name.Text = _anim.Name;
            if (anim.Duration == -1)
            {
                this.tb_popup_date.Visibility = Visibility.Hidden;
                this.tb_popup_starthour.Visibility = Visibility.Hidden;
                this.tb_popup_duration.Visibility = Visibility.Hidden;
                this.tb_title_date.Visibility = Visibility.Hidden;
                this.tb_title_duration.Visibility = Visibility.Hidden;
                this.tb_title_starthour.Visibility = Visibility.Hidden;
                this.tb_perma_anim.Visibility = Visibility.Visible;
                Grid.SetRow(this.scr_desc, 3);
                Grid.SetRowSpan(this.scr_desc, 3);
            }
            else
            {
                this.tb_popup_date.Text = _anim.StartDate.ToString("dd/MM");
                this.tb_popup_starthour.Text = _anim.StartDate.Hour + "h";
                this.tb_popup_duration.Text = _anim.Duration + "h";
            }
            this.tb_popup_host.Text = _anim.Host;
            this.tb_popup_desc.Text = _anim.Description;

            this.animation_border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_anim.ColorString));
            this.tb_animation_name.Text = _anim.Name.ToUpper();

            this.animation_border.MouseLeave += Animation_border_MouseLeave;
            this.animation_border.MouseEnter += Animation_border_MouseEnter;
            this.animation_border.MouseRightButtonDown += Animation_border_MouseRightButtonDown;
            this.animation_border.MouseLeftButtonDown += Animation_border_MouseLeftButtonDown;

            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(1);

            this.animation_border.PreviewMouseWheel += Animation_border_PreviewMouseWheel;
        }

        private void Animation_border_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scrollPos = scr_desc.VerticalOffset - e.Delta;
            if (scrollPos < 0)
                scrollPos = 0;
            if (scrollPos > scr_desc.ScrollableHeight)
                scrollPos = (int)scr_desc.ScrollableHeight;

            if (this.anim_popup.IsOpen)
            {
                scr_desc.ScrollToVerticalOffset(scrollPos);
                e.Handled = true;
            }
        }

        private void Animation_border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_anim.URL != null && _anim.URL != string.Empty)
                System.Diagnostics.Process.Start(_anim.URL);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            this.anim_popup.IsOpen = true;
        }

        private void Animation_border_MouseEnter(object sender, MouseEventArgs e)
        {
            _timer.Start();
        }

        private void Animation_border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (FormExecution.PlayerInfos.Rank < PlayerRank.Animateurs)
                return;

            UpdateAnimation control = new UpdateAnimation(_anim);
            control.Show();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => control.Activate()));
            control.SendUpdate += Control_SendUpdate;
        }

        private void Control_SendUpdate(Animation anim, bool remove)
        {
            _admin.SendAnimationUpdate(anim, remove);
        }

        private void Animation_border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.anim_popup.IsOpen)
                this.anim_popup.IsOpen = false;
            if (_timer.IsEnabled)
                _timer.Stop();
        }
    }
}
