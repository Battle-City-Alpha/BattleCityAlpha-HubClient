using System.Windows.Controls;
using System.Windows.Media;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_BlurLabel.xaml
    /// </summary>
    public partial class BCA_BlurLabel : UserControl
    {
        public BCA_BlurLabel()
        {
            InitializeComponent();
        }

        public void InitLabel(string text, Color front, Color back, int fontsize = -1)
        {
            lblBackground.Content = text;
            lblText.Content = text;

            if (fontsize != -1)
            {
                lblBackground.FontSize = fontsize + 5;
                lblText.FontSize = fontsize;
            }

            lblBackground.Background = new SolidColorBrush(back);
            lblText.Foreground = new SolidColorBrush(front);

        }
    }
}
