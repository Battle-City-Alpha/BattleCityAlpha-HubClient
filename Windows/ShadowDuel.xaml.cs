using BCA.Common;
using BCA.Common.Bets;
using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
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
    /// Logique d'interaction pour ShadowDuel.xaml
    /// </summary>
    public partial class ShadowDuel : Window
    {
        private DuelRequestAdministrator _admin;
        private Bet _bet;
        private BetType _bettype;
        private int _id;
        public ShadowDuel(DuelRequestAdministrator admin, int id)
        {
            InitializeComponent();
            _admin = admin;
            _id = id;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MouseDown += Window_MouseDown;

            rb_BP.IsChecked = true;
            _bettype = BetType.BPs;

            rb_BP.Checked += Rb_BP_Checked;
            rb_ban.Checked += Rb_ban_Checked;
            rb_mute.Checked += Rb_mute_Checked;

            cb_dueltype.Items.Add(RoomType.Single);
            cb_dueltype.Items.Add(RoomType.Match);

            cb_masterrules.Items.Add("MR5 (Avril 2020)");
            cb_masterrules.Items.Add("MR4 (Link)");
            cb_masterrules.Items.Add("MR3 (Pendules)");
            cb_masterrules.Items.Add("MR2 (Synchro/XYZ)");
            cb_masterrules.Items.Add("MR1 (Basique)");

            cb_banlist.ItemsSource = FormExecution.GetBanlists().Keys.ToArray();

            cb_banlist.SelectedIndex = 0;
            cb_dueltype.SelectedIndex = 0;
            tb_handcard.Text = "5";
            tb_lpstartduel.Text = "8000";
            cb_masterrules.SelectedIndex = 0;
            tb_drawcount.Text = "1";
            chb_shuffledeck.IsChecked = false;

            btnChoose.MouseLeftButtonDown += BtnChoose_MouseLeftButtonDown;
            btnSend.MouseLeftButtonDown += BtnSend_MouseLeftButtonDown;

            LoadStyle();
        }

        private void Rb_BP_Checked(object sender, RoutedEventArgs e)
        {
            _bettype = BetType.BPs;
        }

        private void Rb_mute_Checked(object sender, RoutedEventArgs e)
        {
            _bettype = BetType.Mute;
        }

        private void Rb_ban_Checked(object sender, RoutedEventArgs e)
        {
            _bettype = BetType.Ban;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnSend_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int MR = 5 - cb_masterrules.SelectedIndex;
            string password = string.Empty;
            _admin.SendShadowDuelRequest(_id, password, (RoomType)cb_dueltype.SelectedIndex, FormExecution.GetBanlistValue(cb_banlist.SelectedItem.ToString()), RoomRules.TCG, Convert.ToInt32(tb_handcard.Text), Convert.ToInt32(tb_lpstartduel.Text), MR, Convert.ToInt32(tb_drawcount.Text), chb_shuffledeck.IsChecked == true, "Duel des ombres...", _bettype, _bet);
            
            Close();
        }

        private void BtnChoose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((bool)rb_BP.IsChecked || (bool)rb_ban.IsChecked || (bool)rb_mute.IsChecked)
            {
                InputText form = new InputText("mise...");
                form.Title = "Duel des ombres...";
                form.SelectedText += BP_bet;
                form.Show();
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => form.Activate()));
            }
        }
        private void Cb_dueltype_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cb_dueltype.SelectedIndex == 2)
                tb_lpstartduel.Text = "16000";
            else
                tb_lpstartduel.Text = "8000";
        }

        private void BP_bet(string obj)
        {
            int pts;
            if (int.TryParse(obj, out pts))
            {
                switch (_bettype)
                {
                    case BetType.BPs:
                        _bet = new BPsBet(pts);
                        btnChoose.ButtonText = pts + " BPs";
                        btnChoose.Update();
                        break;
                    case BetType.Mute:
                    case BetType.Ban:
                        _bet = new SanctionBet(_bettype, pts);
                        btnChoose.ButtonText = pts + " h";
                        btnChoose.Update();
                        break;
                }
                
                AllowToSend();
            }
            else
               FormExecution.Client_PopMessageBox("Vous n'avez pas indiqué un nombre valable de BPs.", "Erreur");

        }

        private void AllowToSend()
        {
            Grid.SetColumn(btnChoose, 0);
            Grid.SetColumnSpan(btnChoose, 1);
            btnSend.Visibility = Visibility.Visible;
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> RankedButtons = new List<BCA_ColorButton>();
            RankedButtons.AddRange(new[] { btnChoose, btnSend });

            foreach (BCA_ColorButton btn in RankedButtons)
            {
                btn.Color1 = FormExecution.AppDesignConfig.GetGameColor("Color1ShadowDuel");
                btn.Color2 = FormExecution.AppDesignConfig.GetGameColor("Color2ShadowDuel");
                btn.Update();
            }

            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void minimizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(100, 100, 0, 0);
            }
            else if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.bg_border.CornerRadius = new CornerRadius(0);
            }
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
