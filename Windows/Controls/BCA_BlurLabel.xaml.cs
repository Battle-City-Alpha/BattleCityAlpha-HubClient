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
    /// Logique d'interaction pour BCA_BlurLabel.xaml
    /// </summary>
    public partial class BCA_BlurLabel : UserControl
    {
        public BCA_BlurLabel()
        {
            InitializeComponent();
        }

        public void InitLabel(string text, int fontsize = -1)
        {
            lblBackground.Content = text;
            lblText.Content = text;

            if (fontsize != -1)
            {
                lblBackground.FontSize = fontsize;
                lblText.FontSize = fontsize;
            }

        }
    }
}
