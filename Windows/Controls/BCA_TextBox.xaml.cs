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
    /// Logique d'interaction pour BCA_TextBox.xaml
    /// </summary>
    public partial class BCA_TextBox : UserControl
    {
        public BCA_TextBox()
        {
            InitializeComponent();
        }

        public string GetText()
        {
            return tbChat.Text;
        }

        public void Clear()
        {
            tbChat.Clear();
        }
    }
}
