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
        }

        public void AddItem(string username)
        {
            if (!lb.Items.Contains(username))
                lb.Items.Add(username);
        }
        public void RemoveItem(string username)
        {
            if (lb.Items.Contains(username))
                lb.Items.Remove(username);
        }
        public void Clear()
        {
            lb.Items.Clear();
        }
    }
}
