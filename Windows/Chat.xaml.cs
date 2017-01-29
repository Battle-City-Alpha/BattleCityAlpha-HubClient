using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.WindowsAdministrator;
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
    /// Logique d'interaction pour Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        ChatAdministrator _admin;

        public Chat(ChatAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;
            _admin.ChatMessage += _admin_ChatMessage;
        }

        private void _admin_ChatMessage(string msg)
        {
            chat.AppendText(msg);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    StandardClientChatMessage packet = new StandardClientChatMessage(chatTB.Text);
                    FormExecution.Client.Send(PacketType.ChatMessage, packet);
                    chatTB.Clear();
                    break;
            }
            e.Handled = true;
        }
    }
}
