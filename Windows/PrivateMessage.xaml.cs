using BCA.Common;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using hub_client.WindowsAdministrator;
using NLog;
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
    /// Logique d'interaction pour PrivateMessage.xaml
    /// </summary>
    public partial class PrivateMessage : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private PlayerInfo _target;
        PrivateMessageAdministrator _admin;

        public PrivateMessage(string target, PrivateMessageAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;
            _target = _admin.Client.GetPlayerInfo(target);
            if (_target == null)
            {
                Close();
                return;
            }
            Title = "Privé : " + _target.Username;

            _admin.MessageRecieved += _admin_MessageRecieved;
        }

        private void _admin_MessageRecieved(string message)
        {
            Dispatcher.InvokeAsync(delegate { rtbChat.OnColoredMessage(FormExecution.AppDesignConfig.StandardMessageColor, message, false, false); });
            Dispatcher.InvokeAsync(delegate { Show(); });
        }

        private void BCA_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (tbChat.GetText() == string.Empty)
                        return;
                    _admin.SendMessage(_target, tbChat.GetText());
                    tbChat.Clear();
                    break;
            }
            e.Handled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.MessageRecieved -= _admin_MessageRecieved;
        }
    }
}
