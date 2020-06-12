using BCA.Common;
using BCA.Common.Bets;
using hub_client.Cards;
using hub_client.Windows.Controls;
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
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour ShadowDuelRequest.xaml
    /// </summary>
    public partial class ShadowDuelRequest : Window
    {
        public event Action<Bet, bool> Results;

        private Bet _bet;

        public ShadowDuelRequest(PlayerInfo player, RoomConfig config, Bet bet)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            LoadStyle();

            _bet = bet;

            if (_bet is BPsBet)
            {
                btnChoose.Visibility = Visibility.Hidden;
                btnAgree.Visibility = Visibility.Visible;
                btnDisagree.Visibility = Visibility.Visible;
            }
            else
            {
                foreach (PlayerCard card in ((CardsBet)_bet).Cards[player.UserId])
                    lb_cards.Items.Add(card.Name);
            }

            string txt = string.Empty;
            txt = string.Format("Vous avez été invité en duel des ombres par {0}. \r\nType : {1}", player.Username, config.Type);
            txt += Environment.NewLine + string.Format("MasterRules : {0}", config.MasterRules);
            txt += Environment.NewLine + string.Format("Banlist : {0}", FormExecution.GetBanlistValue(config.Banlist));
            txt += Environment.NewLine + string.Format("Point de vie : {0}", config.StartDuelLP);
            txt += Environment.NewLine + string.Format("Carte dans la main au départ : {0}", config.CardByHand);
            txt += Environment.NewLine + string.Format("Pioche par tour : {0}", config.DrawCount);
            txt += Environment.NewLine + string.Format("Info : {0}", config.CaptionText);
            txt += Environment.NewLine + (config.NoShuffleDeck ? "Deck non mélangé" : "Deck mélangé");

            tb_maintext.Text = txt;

            txt = string.Format("Mise : {0}", bet is CardsBet ? "Des cartes" : ((BPsBet)bet).Amount + "BPs");
            tb_mise.Text = txt;

            btnChoose.MouseLeftButtonDown += ChooseCards;
            btnAgree.MouseLeftButtonDown += (sender, e) => RequestResult(sender, e, true);
            btnDisagree.MouseLeftButtonDown += (sender, e) => RequestResult(sender, e, false);
        }
        private void LoadStyle()
        {
            List<BCA_ColorButton> RankedButtons = new List<BCA_ColorButton>();
            RankedButtons.AddRange(new[] { btnChoose, btnAgree, btnDisagree });

            foreach (BCA_ColorButton btn in RankedButtons)
            {
                btn.Color1 = FormExecution.AppDesignConfig.GetGameColor("Color1ShadowDuel");
                btn.Color2 = FormExecution.AppDesignConfig.GetGameColor("Color2ShadowDuel");
                btn.Update();
            }

            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }

        private void ChooseCards(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RequestResult(object sender, MouseButtonEventArgs e, bool result)
        {
            Results?.Invoke(_bet, result);
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
