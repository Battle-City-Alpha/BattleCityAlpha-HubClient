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
    /// Logique d'interaction pour InputText.xaml
    /// </summary>
    public partial class InputText : Window
    {
        public event Action<string> SelectedText;

        public InputText()
        {
            InitializeComponent();
        }

        private void btnAgree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tb_text.GetText() == "")
                return;

            SelectedText?.Invoke(tb_text.GetText());
            Close();
        }
    }
}
