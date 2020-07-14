using BCA.Common;
using BCA.Network.Packets.Enums;
using hub_client.Assets;
using hub_client.Stuff;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_Chat.xaml
    /// </summary>
    public partial class BCA_Chat : UserControl
    {
        Popup smiley_popup = new Popup();
        public BCA_Chat()
        {
            InitializeComponent();
            Clear();
            LoadPopup();
        }

        public void LoadPopup()
        {
            smiley_popup.AllowsTransparency = true;
            smiley_popup.Placement = PlacementMode.MousePoint;
            smiley_popup.Margin = new Thickness(0, 0, 0, 0);
            smiley_popup.PopupAnimation = PopupAnimation.Fade;

            Border popup_border = new Border();
            popup_border.CornerRadius = new CornerRadius(5);
            popup_border.Margin = new Thickness(0);
            popup_border.Padding = new Thickness(3);
            popup_border.BorderThickness = new Thickness(1);
            popup_border.BorderBrush = new SolidColorBrush(Colors.Black);
            popup_border.Background = new SolidColorBrush(Colors.White);
            popup_border.Background.Opacity = 0.9;

            TextBlock smiley_text = new TextBlock();
            smiley_text.FontSize = FormExecution.AppDesignConfig.FontSize;
            smiley_text.FontFamily = FormExecution.AppDesignConfig.Font;
            smiley_text.TextAlignment = TextAlignment.Center;

            popup_border.Child = smiley_text;

            smiley_popup.Child = popup_border;
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
                    string url = word;
                    if (word.EndsWith("."))
                        url = word.Substring(0, word.Length - 2);

                    Hyperlink textLink = new Hyperlink(new Run(url));
                    textLink.NavigateUri = new Uri(url);
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
            if (player.Team != 0 && FormExecution.ClientConfig.DisplayTagTeam)            
                pl.Text += " (" + player.TeamTag + ")";
            
            pl.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + player.ChatColorString));

            Run txt = new Run("]: ");
            txt.Foreground = new SolidColorBrush(color);

            /*Image avatar = new Image();
            avatar.Width = FormExecution.AppDesignConfig.FontSize + 10;
            avatar.Height = FormExecution.AppDesignConfig.FontSize + 10;
            avatar.Source = DrawingImageToBitmapImage(RoundCorners(FormExecution.AssetsManager.GetCustom(player.Avatar), 300, System.Drawing.Color.Transparent));
            avatar.Margin = new Thickness(3);*/

            date.BaselineAlignment = BaselineAlignment.Center;
            pl.BaselineAlignment = BaselineAlignment.Center;
            txt.BaselineAlignment = BaselineAlignment.Center;
            //pr.Inlines.Add(avatar);
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
                normalTxt.BaselineAlignment = BaselineAlignment.Center;

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
                    string url = word;
                    if (word.EndsWith("."))
                        url = word.Substring(0, word.Length - 2);

                    Hyperlink textLink = new Hyperlink(new Run(url));
                    if (highlight)
                        textLink.Background = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("HighlighMessageColor"));
                    textLink.NavigateUri = new Uri(url);
                    textLink.RequestNavigate += TextLink_RequestNavigate;

                    textLink.BaselineAlignment = BaselineAlignment.Center;

                    pr.Inlines.Add(textLink);

                    normalTxt.Text = " ";
                }
                else if (word.Length > 2 && word.StartsWith(":") && word.EndsWith(":") && FormExecution.AssetsManager.CheckSmiley(word.Substring(1, word.Length-2)) != null)
                {
                    Image img = new Image();
                    img.Source = FormExecution.AssetsManager.CheckSmiley(word.Substring(1, word.Length - 2)).Pic.Source.Clone();
                    img.Width = FormExecution.AppDesignConfig.FontSize + 10;
                    img.Height = FormExecution.AppDesignConfig.FontSize + 10;
                    img.MouseEnter += (sender, e) => Smiley_Hover(sender, e, word);
                    img.MouseLeave += Smiley_Leave;
                    pr.Inlines.Add(img);
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

                normalTxt.BaselineAlignment = BaselineAlignment.Center;
                normalTxt.Foreground = new SolidColorBrush(color);
                pr.Inlines.Add(normalTxt);
            }

            pr.Margin = new Thickness(0);
            chat.Document.Blocks.Add(pr);


            if (FormExecution.ClientConfig.Autoscroll)
                ScrollToCarret();
        }

        private void Smiley_Leave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            smiley_popup.IsOpen = false;
        }

        private void Smiley_Hover(object sender, System.Windows.Input.MouseEventArgs e, string txt)
        {
            ((smiley_popup.Child as Border).Child as TextBlock).Text = txt;
            smiley_popup.IsOpen = true;
        }

        public void ShowSmileys()
        {
            Paragraph pr = new Paragraph();
            Run date = new Run("[" + DateTime.Now.ToString("HH:mm") + "] [");
            date.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor"));


            Run pl = new Run("Smiley]: ");
            pl.Foreground = new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor"));

            date.BaselineAlignment = BaselineAlignment.Center;
            pl.BaselineAlignment = BaselineAlignment.Center;
            pr.Inlines.Add(date);
            pr.Inlines.Add(pl);

            foreach (var info in FormExecution.AssetsManager.Smileys)
            {
                pr.Inlines.Add("Groupe : " + info.Key + " |");
                foreach (Smiley s in info.Value)
                {
                    pr.Inlines.Add(":" + s.Name + ":");
                    pr.Inlines.Add("   -   ");
                    Image img = new Image();
                    img.Source = s.Pic.Source.Clone();
                    img.Width = FormExecution.AppDesignConfig.FontSize + 10;
                    img.Height = FormExecution.AppDesignConfig.FontSize + 10;
                    pr.Inlines.Add(img);
                    pr.Inlines.Add(" | ");
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

        public System.Drawing.Image RoundCorners(BitmapImage StartImage, int CornerRadius, System.Drawing.Color BackgroundColor)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(StartImage.UriSource.LocalPath);

            CornerRadius *= 2;
            System.Drawing.Bitmap RoundedImage = new System.Drawing.Bitmap(img.Width, img.Height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(RoundedImage))
            {
                g.Clear(BackgroundColor);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Brush brush = new System.Drawing.TextureBrush(img);
                System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                gp.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
                gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
                gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                gp.AddArc(0, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
                g.FillPath(brush, gp);
                return RoundedImage;
            }
        }
        private BitmapImage DrawingImageToBitmapImage(System.Drawing.Image image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;

                BitmapImage pngImage = new System.Windows.Media.Imaging.BitmapImage();
                pngImage.BeginInit();
                pngImage.CacheOption = BitmapCacheOption.OnLoad;
                pngImage.StreamSource = stream;
                pngImage.EndInit();

                return pngImage;
            }
        }
    }
}