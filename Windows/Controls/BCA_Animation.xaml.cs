using BCA.Common;
using BCA.Network.Packets.Enums;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            this.tb_popup_date.Text = _anim.StartDate.ToString("dd/MM");
            this.tb_popup_starthour.Text = _anim.StartDate.Hour + "h";
            this.tb_popup_duration.Text = _anim.Duration + "h";
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
        }

        private void Animation_border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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
