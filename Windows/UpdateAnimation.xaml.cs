using BCA.Common;
using hub_client.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour UpdateAnimation.xaml
    /// </summary>
    public partial class UpdateAnimation : Window
    {
        private int id;

        public event Action<Animation, bool> SendUpdate;
        public UpdateAnimation(Animation anim)
        {
            InitializeComponent();

            if (anim != null)
            {
                id = anim.ID;
                tb_animation_name.Text = anim.Name;                
                tb_duration.Text = anim.Duration.ToString();
                tb_host.Text = anim.Host;
                tb_url.Text = anim.URL;
                dp_startdate.Value = anim.StartDate;
                rtb_desc.AppendText(anim.Description);
                chk_perma.IsChecked = anim.Duration == -1;

                tb_duration.IsEnabled = chk_perma.IsChecked == false;
                dp_startdate.IsEnabled = chk_perma.IsChecked == false;

                btn_update.ButtonText = "Modifier";
                btn_update.Update();
            }
            else
            {
                id = -1;
                tb_host.Text = FormExecution.PlayerInfos.Username;
                btn_delete.IsEnabled = false;
            }

            this.MouseDown += Window_MouseDown;

            this.btn_update.MouseLeftButtonDown += Btn_update_MouseLeftButtonDown;
            this.btn_delete.MouseLeftButtonDown += Btn_delete_MouseLeftButtonDown;

            this.chk_perma.Checked += Chk_perma_Checked;
            this.chk_perma.Unchecked += Chk_perma_Unchecked;

            this.tb_duration.PreviewTextInput += NumberValidationTextBox;

            LoadStyle();
        }

        private void Chk_perma_Unchecked(object sender, RoutedEventArgs e)
        {
            tb_duration.IsEnabled = true;
            dp_startdate.IsEnabled = true;
        }

        private void Chk_perma_Checked(object sender, RoutedEventArgs e)
        {
            tb_duration.IsEnabled = false;
            dp_startdate.IsEnabled = false;
        }

        public void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_update, btn_delete });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = FormExecution.AppDesignConfig.GetGameColor("Color1AnimationPlanning");
                btn.Color2 = FormExecution.AppDesignConfig.GetGameColor("Color2AnimationPlanning");
                btn.Update();
            }

            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }

        private void Btn_delete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SendUpdate?.Invoke(new Animation
            {
                ID = id,
                Host = tb_host.Text,
                Name = tb_animation_name.Text,
                Duration = chk_perma.IsChecked == true ? -1 : Convert.ToInt32(tb_duration.Text),
                Description = new TextRange(rtb_desc.Document.ContentStart, rtb_desc.Document.ContentEnd).Text,
                URL = tb_url.Text,
                StartDate = chk_perma.IsChecked == true ? DateTime.Now :(DateTime)dp_startdate.Value
            }, true);
            Close();
        }

        private void Btn_update_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SendUpdate?.Invoke(new Animation
            {
                ID = id,
                Host = tb_host.Text,
                Name = tb_animation_name.Text,
                Duration = chk_perma.IsChecked == true ? -1 : Convert.ToInt32(tb_duration.Text),
                Description = new TextRange(rtb_desc.Document.ContentStart, rtb_desc.Document.ContentEnd).Text,
                URL = tb_url.Text,
                StartDate = chk_perma.IsChecked == true ? DateTime.Now : (DateTime)dp_startdate.Value
            }, false);
            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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
