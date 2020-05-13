using BCA.Common;
using BCA.Network.Packets.Enums;
using System;
using System.Diagnostics;
using System.Linq;
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
            this.chat.VerticalScrollBarVisibility = FormExecution.ClientConfig.ShowChatScrollbar ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
        }

        public void OnSpecialColoredMessage(Color color, string text)
        {
            Paragraph pr = new Paragraph();
            Run date = new Run("[" + DateTime.Now.ToString("HH:mm") + "] ");
            date.Foreground = new SolidColorBrush(color);
            pr.Inlines.Add(date);

            string[] args = text.Split(' ');

            bool bold = false;
            bool italic = false;
            bool underline = false;
            foreach (string word in args)
            {
                Run normalTxt = new Run();

                if (bold)
                    normalTxt.FontWeight = FontWeights.Bold;
                if (italic)
                    normalTxt.FontStyle = FontStyles.Italic;
                if (underline)
                    normalTxt.TextDecorations = TextDecorations.Underline;

                if (word.StartsWith("http://") || word.StartsWith("www.") || word.StartsWith("https://"))
                {
                    Hyperlink textLink = new Hyperlink(new Run(word));
                    textLink.NavigateUri = new Uri(word);
                    textLink.RequestNavigate += TextLink_RequestNavigate;

                    pr.Inlines.Add(textLink);

                    normalTxt.Text = " ";
                }
                else if (word == "**")
                {
                    bold = !bold;
                    continue;
                }
                else if (word == "*")
                {
                    italic = !italic;
                    continue;
                }
                else if (word == "__")
                {
                    underline = !underline;
                    continue;
                }
                else if (word.StartsWith("**"))
                {
                    normalTxt.Text = (word.Substring(2) + " ");
                    normalTxt.FontWeight = FontWeights.Bold;
                    bold = true;
                }
                else if (word.EndsWith("**"))
                {
                    normalTxt.Text = (word.Substring(0, word.Length - 2) + " ");
                    bold = false;
                }
                else if (word.StartsWith("*"))
                {
                    normalTxt.Text = (word.Substring(1) + " ");
                    normalTxt.FontStyle = FontStyles.Italic;
                    italic = true;
                }
                else if (word.EndsWith("*"))
                {
                    normalTxt.Text = (word.Substring(0, word.Length - 1) + " ");
                    italic = false;
                }
                else if (word.StartsWith("__"))
                {
                    normalTxt.Text = (word.Substring(2) + " ");
                    normalTxt.TextDecorations = TextDecorations.Underline;
                    underline = true;
                }
                else if (word.EndsWith("__"))
                {
                    normalTxt.Text = (word.Substring(0, word.Length - 2) + " ");
                    underline = false;
                }
                else if (word == FormExecution.Username)
                {
                    normalTxt.Text = (word + " ");
                    normalTxt.FontWeight = FontWeights.Bold;

                    FormExecution.FlashChat();
                }
                else
                {
                    normalTxt.Text = (word + " ");
                }

                normalTxt.Foreground = new SolidColorBrush(color);
                pr.Inlines.Add(normalTxt);
            }

            pr.Margin = new Thickness(0);
            chat.Document.Blocks.Add(pr);


            if (FormExecution.ClientConfig.Autoscroll)
                ScrollToCarret();
        }
        public void OnPlayerColoredMessage(Color color, PlayerInfo player, string text, bool canHighlight = false)
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
            if (!canHighlight && args.Contains(FormExecution.Username))
                highlight = true;

            bool bold = false;
            bool italic = false;
            bool underline = false;
            foreach (string word in args)
            {
                Run normalTxt = new Run();

                if (bold)
                    normalTxt.FontWeight = FontWeights.Bold;
                if (italic)
                    normalTxt.FontStyle = FontStyles.Italic;
                if (underline)
                    normalTxt.TextDecorations = TextDecorations.Underline;
                if (highlight)
                    normalTxt.Background = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("HighlighMessageColor"));

                if (word.StartsWith("http://") || word.StartsWith("www.") || word.StartsWith("https://"))
                {
                    Hyperlink textLink = new Hyperlink(new Run(word));
                    if (highlight)
                        textLink.Background = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("HighlighMessageColor"));
                    textLink.NavigateUri = new Uri(word);
                    textLink.RequestNavigate += TextLink_RequestNavigate;

                    pr.Inlines.Add(textLink);

                    normalTxt.Text = " ";
                }
                else if (word == "**")
                {
                    bold = !bold;
                    continue;
                }
                else if (word == "*")
                {
                    italic = !italic;
                    continue;
                }
                else if (word == "__")
                {
                    underline = !underline;
                    continue;
                }
                else if (word.StartsWith("**"))
                {
                    normalTxt.Text = (word.Substring(2) + " ");
                    normalTxt.FontWeight = FontWeights.Bold;
                    bold = true;
                }
                else if (word.EndsWith("**"))
                {
                    normalTxt.Text = (word.Substring(0, word.Length - 2) + " ");
                    bold = false;
                }
                else if (word.StartsWith("*"))
                {
                    normalTxt.Text = (word.Substring(1) + " ");
                    normalTxt.FontStyle = FontStyles.Italic;
                    italic = true;
                }
                else if (word.EndsWith("*"))
                {
                    normalTxt.Text = (word.Substring(0, word.Length - 1) + " ");
                    italic = false;
                }
                else if (word.StartsWith("__"))
                {
                    normalTxt.Text = (word.Substring(2) + " ");
                    normalTxt.TextDecorations = TextDecorations.Underline;
                    underline = true;
                }
                else if (word.EndsWith("__"))
                {
                    normalTxt.Text = (word.Substring(0, word.Length - 2) + " ");
                    underline = false;
                }
                else if (word == FormExecution.Username)
                {
                    normalTxt.Text = (word + " "); 
                    normalTxt.FontWeight = FontWeights.Bold;

                    FormExecution.FlashChat();
                }
                else
                {
                    normalTxt.Text = (word + " ");               
                }

                normalTxt.Foreground = new SolidColorBrush(color);
                pr.Inlines.Add(normalTxt);
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
