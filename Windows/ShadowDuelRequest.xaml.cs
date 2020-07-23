using BCA.Common;
using BCA.Common.Bets;
using BCA.Common.Enums;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour ShadowDuelRequest.xaml
    /// </summary>
    public partial class ShadowDuelRequest : Window
    {
        public event Action<Bet, bool> Results;

        private Bet _bet;
        private DuelRequestAdministrator _admin;

        public ShadowDuelRequest(PlayerInfo player, RoomConfig config, Bet bet, DuelRequestAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MouseDown += Window_MouseDown;

            LoadStyle();

            _bet = bet;
            _admin = admin;

            btnChoose.Visibility = Visibility.Hidden;
            btnAgree.Visibility = Visibility.Visible;
            lbPanel.Visibility = Visibility.Hidden;

            tb_popup_banlist.Foreground = new SolidColorBrush(Colors.White);
            tb_popup_lp.Foreground = new SolidColorBrush(Colors.White);
            tb_popup_MR.Foreground = new SolidColorBrush(Colors.White);
            tb_popup_starthand.Foreground = new SolidColorBrush(Colors.White);
            tb_shuffledeck.Foreground = new SolidColorBrush(Colors.White);
            tb_drawcount.Foreground = new SolidColorBrush(Colors.White);

            if (config.Banlist != 0)
                tb_popup_banlist.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
            tb_popup_banlist.Text = FormExecution.GetBanlistValue(config.Banlist);

            if ((config.StartDuelLP != 8000 && config.Type != RoomType.Tag) || (config.StartDuelLP != 16000 && config.Type == RoomType.Tag))
                tb_popup_lp.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
            tb_popup_lp.Text = config.StartDuelLP.ToString();

            if (config.MasterRules != 5)
                tb_popup_MR.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
            tb_popup_MR.Text = config.MasterRules.ToString();

            if (config.CardByHand != 5)
                tb_popup_starthand.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
            tb_popup_starthand.Text = config.CardByHand.ToString();

            if (config.DrawCount != 1)
                tb_drawcount.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
            tb_drawcount.Text = config.DrawCount.ToString();

            if (config.NoShuffleDeck)
            {
                tb_shuffledeck.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor"));
                tb_shuffledeck.Text = "Deck non mélangé";
            }

            tb_popup_type.Text = config.Type.ToString();

            tb_captiontext.Text = config.CaptionText;
            tb_title.Text = string.Format("Vous avez été invité en duel des ombres par {0}", player.Username);


            switch (bet.BType)
            {
                case BetType.BPs:
                    tb_mise.Text = string.Format("{0}", ((BPsBet)bet).Amount + "BPs");
                    break;
                case BetType.Ban:
                    tb_mise.Text = string.Format("{0}", "Ban - " + ((SanctionBet)bet).Time + "H");
                    break;
                case BetType.Mute:
                    tb_mise.Text = string.Format("{0}", "Mute - " + ((SanctionBet)bet).Time + "H");
                    break;
            }

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

        private void RequestResult(object sender, MouseButtonEventArgs e, bool result)
        {
            Results?.Invoke(_bet, result);
            Close();
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
