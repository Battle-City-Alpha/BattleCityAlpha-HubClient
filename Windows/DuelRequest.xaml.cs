using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour DuelRequest.xaml
    /// </summary>
    public partial class DuelRequest : Window
    {
        private DuelRequestAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        private int _id;

        public DuelRequest(DuelRequestAdministrator admin, int id)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _admin = admin;

            cb_dueltype.ItemsSource = Enum.GetValues(typeof(RoomType)).Cast<RoomType>();

            cb_masterrules.Items.Add("MR5 (Avril 2020)");
            cb_masterrules.Items.Add("MR4 (Link)");
            cb_masterrules.Items.Add("MR3 (Pendules)");
            cb_masterrules.Items.Add("MR2 (Synchro/XYZ)");
            cb_masterrules.Items.Add("MR1 (Basique)");

            _id = id;

            this.MouseDown += Window_MouseDown;

            tb_captiontext.GotFocus += Tb_captiontext_GotFocus;

            chb_password.Unchecked += chb_password_Checked;
        }

        private void Tb_captiontext_GotFocus(object sender, RoutedEventArgs e)
        {
            tb_captiontext.Text = "";
        }

        private void LoadStyle()
        {
            btnSend.Color1 = style.GetGameColor("Color1DuelRequestButton");
            btnSend.Color2 = style.GetGameColor("Color2DuelRequestButton");
            btnSend.Update();

            this.FontFamily = style.Font;
            this.FontSize = style.FontSize;
        }

        private void BtnSend_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int MR = 5 - cb_masterrules.SelectedIndex;
            string password = chb_password.IsChecked == true ? tb_password.Text : string.Empty;
            if (_id != -1)
                _admin.SendRequest(_id, password, (RoomType)cb_dueltype.SelectedIndex, FormExecution.GetBanlistValue(cb_banlist.SelectedItem.ToString()), RoomRules.TCG, Convert.ToInt32(tb_handcard.Text), Convert.ToInt32(tb_lpstartduel.Text), MR, Convert.ToInt32(tb_drawcount.Text), chb_shuffledeck.IsChecked == true, tb_captiontext.Text);
            else
                _admin.SendHost((RoomType)cb_dueltype.SelectedIndex, password, FormExecution.GetBanlistValue(cb_banlist.SelectedItem.ToString()), RoomRules.TCG, Convert.ToInt32(tb_handcard.Text), Convert.ToInt32(tb_lpstartduel.Text), MR, Convert.ToInt32(tb_drawcount.Text), chb_shuffledeck.IsChecked == true, tb_captiontext.Text);

            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();

            cb_banlist.ItemsSource = FormExecution.GetBanlists().Keys.ToArray();

            cb_banlist.SelectedIndex = 0;
            cb_dueltype.SelectedIndex = 0;
            tb_handcard.Text = "5";
            tb_lpstartduel.Text = "8000";
            cb_masterrules.SelectedIndex = 0;
            tb_drawcount.Text = "1";
            chb_shuffledeck.IsChecked = false;
        }


        private void Cb_dueltype_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cb_dueltype.SelectedIndex == 2)
                tb_lpstartduel.Text = "16000";
            else
                tb_lpstartduel.Text = "8000";
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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

        private void chb_password_Checked(object sender, RoutedEventArgs e)
        {
            tb_password.IsEnabled = (bool)chb_password.IsChecked;
        }
    }
}
