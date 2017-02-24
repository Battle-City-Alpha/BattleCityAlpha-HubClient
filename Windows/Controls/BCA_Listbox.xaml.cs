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
    /// Logique d'interaction pour BCA_Listbox.xaml
    /// </summary>
    public partial class BCA_Listbox : UserControl
    {
        public BCA_Listbox()
        {
            InitializeComponent();
            lb.Items.Add("Tic-Tac-Toc");
        }
    }
}
