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
        private int _id;
        public ShadowDuel(DuelRequestAdministrator admin, int id)
        {
            InitializeComponent();
            _admin = admin;
            _id = id;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MouseDown += Window_MouseDown;

            rb_BP.IsChecked = true; 
            
            cb_dueltype.ItemsSource = Enum.GetValues(typeof(RoomType)).Cast<RoomType>();

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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnSend_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int MR = 5 - cb_masterrules.SelectedIndex;
            string password = string.Empty;
            _admin.SendRequest(_id, password, (RoomType)cb_dueltype.SelectedIndex, FormExecution.GetBanlistValue(cb_banlist.SelectedItem.ToString()), RoomRules.TCG, Convert.ToInt32(tb_handcard.Text), Convert.ToInt32(tb_lpstartduel.Text), MR, Convert.ToInt32(tb_drawcount.Text), chb_shuffledeck.IsChecked == true, "Duel des ombres...", _bet);
            
            Close();
        }

        private void BtnChoose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((bool)rb_BP.IsChecked)
            {
                InputText form = new InputText();
                form.Title = "Duel des ombres...";
                form.SelectedText += BP_bet;
                form.ShowDialog();
            }
            else
            {
                _admin.AskSelectCard(AskCollectionReason.GiveCard);
                GiveCard window = new GiveCard(_admin.Client.GiveCardAdmin);
                window.SelectedCards += Window_SelectedCards;
                window.Show();
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => window.Activate()));
            }
        }
        private void Window_SelectedCards(Dictionary<int, PlayerCard> cards)
        {
            _bet = new CardsBet();
            ((CardsBet)_bet).SetCards(cards.Values.ToArray(), FormExecution.PlayerInfos.UserId);
            AllowToSend();
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
                _bet = new BPsBet
                {
                    Amount = pts
                };
                AllowToSend();
            }
            else
               FormExecution.Client_PopMessageBox("Vous n'avez pas indiqué un nombre valable de BPs.", "Erreur", false);

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
