using BCA.Common;
using BCA.Network.Packets.Enums;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
            this.chat.VerticalScrollBarVisibility = FormExecution.ClientConfig.ShowChatScrollbar ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
        }

        public void OnSpecialColoredMessage(Color color, string text, bool IsBold, bool IsItalic)
        {
            Paragraph pr = new Paragraph();
            Run date = new Run("[" + DateTime.Now.ToString("HH:mm") + "] ");
            date.Foreground = new SolidColorBrush(color);
            pr.Inlines.Add(date);

            string[] args = text.Split(' ');
            foreach (string word in args)
            {
                if (word.StartsWith("http://") || word.StartsWith("www.") || word.StartsWith("https://"))
                {
                    Hyperlink textLink = new Hyperlink(new Run(word));
                    textLink.NavigateUri = new Uri(word);
                    textLink.RequestNavigate += TextLink_RequestNavigate;
                    if (IsBold)
                        textLink.FontWeight = FontWeights.Bold;
                    if (IsItalic)
                        textLink.FontStyle = FontStyles.Italic;

                    pr.Inlines.Add(textLink);

                    Run normalTxt = new Run(" ");
                    normalTxt.Foreground = new SolidColorBrush(color);
                    if (IsBold)
                        normalTxt.FontWeight = FontWeights.Bold;
                    if (IsItalic)
                        normalTxt.FontStyle = FontStyles.Italic;
                    pr.Inlines.Add(normalTxt);
                }
                else
                {
                    Run normalTxt = new Run(word + " ");
                    normalTxt.Foreground = new SolidColorBrush(color);
                    if (IsBold)
                        normalTxt.FontWeight = FontWeights.Bold;
                    if (IsItalic)
                        normalTxt.FontStyle = FontStyles.Italic;
                    pr.Inlines.Add(normalTxt);
                }
            }

            pr.Margin = new Thickness(0);
            chat.Document.Blocks.Add(pr);

            if (FormExecution.ClientConfig.Autoscroll)
                ScrollToCarret();
        }
        public void OnPlayerColoredMessage(Color color, PlayerInfo player, string text, bool PM = false)
        {
            Paragraph pr = new Paragraph();
            Run date = new Run("[" + DateTime.Now.ToString("HH:mm") + "] [");
            date.Foreground = new SolidColorBrush(color);


            Run pl = new Run(ParseUsername(player.Username, player.Rank, player.VIP));
            pl.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + player.ChatColorString));

            Run txt = new Run("]: ");
            txt.Foreground = new SolidColorBrush(color);

            pr.Inlines.Add(date);
            pr.Inlines.Add(pl);
            pr.Inlines.Add(txt);

            string[] args = text.Split(' ');

            bool highlight = false;
            if (!PM && args.Contains(FormExecution.Username))
                highlight = true;

            foreach (string word in args)
            {
                if (word.StartsWith("http://") || word.StartsWith("www.") || word.StartsWith("https://"))
                {
                    Hyperlink textLink = new Hyperlink(new Run(word));
                    if (highlight)
                        textLink.Background = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("HighlighMessageColor"));
                    textLink.NavigateUri = new Uri(word);
                    textLink.RequestNavigate += TextLink_RequestNavigate;

                    pr.Inlines.Add(textLink);

                    Run normalTxt = new Run(" ");
                    normalTxt.Foreground = new SolidColorBrush(color);
                    if (highlight)
                        normalTxt.Background = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("HighlighMessageColor"));
                    pr.Inlines.Add(normalTxt);
                }
                else if (word == FormExecution.Username)
                {
                    Run normalTxt = new Run(word + " ");
                    normalTxt.Foreground = new SolidColorBrush(color);
                    normalTxt.Background = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("HighlighMessageColor"));
                    normalTxt.FontWeight = FontWeights.Bold;
                    pr.Inlines.Add(normalTxt);

                    FormExecution.FlashChat();
                }
                else
                {
                    Run normalTxt = new Run(word + " ");
                    normalTxt.Foreground = new SolidColorBrush(color);
                    if (highlight)
                        normalTxt.Background = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("HighlighMessageColor"));
                    pr.Inlines.Add(normalTxt);
                }
            }

            pr.Margin = new Thickness(0);
            chat.Document.Blocks.Add(pr);


            if (FormExecution.ClientConfig.Autoscroll)
                ScrollToCarret();
        }

        private void TextLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
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
