using BCA.Common;
using BCA.Network.Packets.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_Chat.xaml
    /// </summary>
    public partial class BCA_Chat : UserControl
    {
        public BCA_Chat()
        {
            InitializeComponent();
            Clear();
        }
        public void RefreshStyle()
        {
            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }

        public void OnSpecialColoredMessage(Color color, string text, bool IsBold, bool IsItalic)
        {
            Paragraph pr = new Paragraph();
            Run date = new Run("[" + DateTime.Now.ToString("HH:mm") + "] ");
            date.Foreground = new SolidColorBrush(color);
            pr.Inlines.Add(date);

            Run txt = new Run(text);
            txt.Foreground = new SolidColorBrush(color);
            if (IsBold)
                txt.FontWeight = FontWeights.Bold;
            if (IsItalic)
                txt.FontStyle = FontStyles.Italic;

            pr.Inlines.Add(txt);

            pr.Margin = new Thickness(0);
            chat.Document.Blocks.Add(pr);

            if (FormExecution.ClientConfig.Autoscroll)
                ScrollToCarret();
        }
        public void OnPlayerColoredMessage(Color color, PlayerInfo player, string text)
        {
            Paragraph pr = new Paragraph();
            Run date = new Run("[" + DateTime.Now.ToString("HH:mm") + "] [");
            date.Foreground = new SolidColorBrush(color);
            pr.Inlines.Add(date);


            Run pl = new Run(ParseUsername(player.Username, player.Rank, player.VIP));
            pl.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + player.ChatColorString));
            pr.Inlines.Add(pl);

            Run txt = new Run("]: " + text);
            txt.Foreground = new SolidColorBrush(color);
            pr.Inlines.Add(txt);

            pr.Margin = new Thickness(0);
            chat.Document.Blocks.Add(pr);

            if (FormExecution.ClientConfig.Autoscroll)
                ScrollToCarret();
        }

        public void ScrollToCarret()
        {
            this.chat.ScrollToEnd();
        }

        public void Clear()
        {
            chat.Document.Blocks.Clear();
        }

        public void SetText(string txt)
        {
            chat.Document.Blocks.Clear();
            chat.Document.Blocks.Add(new Paragraph(new Run(txt)));
        }

        public string GetText()
        {
            return new TextRange(chat.Document.ContentStart, chat.Document.ContentEnd).Text;
        }
        public string ParseUsername(string username, PlayerRank rank, bool isVip)
        {
            /*switch (rank)
            {
                case PlayerRank.Owner:
                    return "♛" + username;
                case PlayerRank.Bot:
                    return "☎" + username;
                case PlayerRank.Moderateurs:
                    return "♝" + username;
                case PlayerRank.Animateurs:
                    return "♞" + username;
                case PlayerRank.Developper:
                    return "♣" + username;
                case PlayerRank.Contributor:
                    return "♟" + username;
                default: */
            if (isVip)
                return "✮" + username;
            else
                return username;
        }
    }
}
