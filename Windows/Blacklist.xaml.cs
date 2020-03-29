using BCA.Common;
using hub_client.Configuration;
using hub_client.Helpers;
using hub_client.Windows.Controls;
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
    /// Logique d'interaction pour Blacklist.xaml
    /// </summary>
    public partial class Blacklist : Window
    {
        public BlacklistManager Manager;

        public Blacklist(BlacklistManager manager)
        {
            InitializeComponent();
            Manager = manager;

            foreach (PlayerInfo player in Manager.Blacklist)
                lbBlacklist.Items.Add(player.Username);

            LoadStyle();
        }

        private void btnRetire_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PlayerInfo info = Manager.Blacklist[lbBlacklist.SelectedIndex];
                lbBlacklist.SelectedItem = null;
                lbBlacklist.Items.Remove(info.Username);
                Manager.Blacklist.Remove(info);
                Manager.Save();
            }
            catch (Exception ex)
            {
                
            }
        }
        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btnRetire });

            AppDesignConfig style = FormExecution.AppDesignConfig;

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.Color1ToolsButton;
                btn.Color2 = style.Color2ToolsButton;
                btn.Update();
            }
        }
    }
}
