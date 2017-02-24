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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        public void OnColoredMessage(Color color, string text, bool IsBold, bool IsItalic)
        {
            Brush b = new SolidColorBrush(color);
            TextRange tr = new TextRange(chat.Document.ContentEnd, chat.Document.ContentEnd);
            tr.Text = text;
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, b);
            tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            tr.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
            if (IsBold)
                tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            if (IsItalic)
                tr.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
            this.chat.AppendText(Environment.NewLine);
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
    }
}
