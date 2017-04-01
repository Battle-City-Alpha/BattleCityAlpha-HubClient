using BCA.Network.Packets;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Configuration;
using hub_client.Helpers;
using hub_client.Network;
using hub_client.Windows.Controls;
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
        bool _animationChat = false;
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        private ChatCommandParser _cmdParser;

        public Chat(ChatAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;
            _cmdParser = new ChatCommandParser();

            _admin.ChatMessage += _admin_ChatMessage;
            this.Loaded += Chat_Loaded;
            _admin.LoginComplete += _admin_LoginComplete;
            _admin.AddHubPlayer += _admin_AddHubPlayer;
            _admin.RemoveHubPlayer += _admin_RemoveHubPlayer;
        }

        private void _admin_RemoveHubPlayer(string username)
        {
            Dispatcher.InvokeAsync(delegate
            {
                lbUserlist.RemoveItem(username);
            });
        }

        private void _admin_AddHubPlayer(string username)
        {
            Dispatcher.InvokeAsync(delegate
            {
                lbUserlist.AddItem(username);
            });
        }

        private void _admin_LoginComplete()
        {
            Show();
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> HeadButtons = new List<BCA_ColorButton>();
            HeadButtons.AddRange(new[] { btnArene, btnShop, btnDecks, btnRanking, btnTools });
            List<BCA_ColorButton> BottomButtons = new List<BCA_ColorButton>();
            BottomButtons.AddRange(new[] { btnProfil, btnFAQ, btnReplay, btnNote, btnDiscord });

            foreach(BCA_ColorButton btn in HeadButtons)
            {
                btn.Color1 = style.Color1HomeHeadButton;
                btn.Color2 = style.Color2HomeHeadButton;
                btn.Update();
            }
            foreach (BCA_ColorButton btn in BottomButtons)
            {
                btn.Color1 = style.Color1HomeBottomButton;
                btn.Color2 = style.Color2HomeBottomButton;
                btn.Update();
            }
            btnChannel.Color1 = style.Color1HomePlaceButton;
            btnChannel.Color2 = style.Color2HomePlaceButton;
            btnChannel.Update();
        }

        private void _admin_ChatMessage(Color c, string msg, bool italic, bool bold)
        {
            Dispatcher.InvokeAsync(delegate { chat.OnColoredMessage(c, msg, italic, bold); });
        }

        private void BtnAnimations_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            chat.Clear();
            if (_animationChat)
            {
                btnChannel.ButtonText = "Place";
                btnChannel.Color1 = (Color)ColorConverter.ConvertFromString("#FFEA1313");
                btnChannel.Color2 = (Color)ColorConverter.ConvertFromString("#FF5F0B08");
                chat.OnColoredMessage(Colors.DarkViolet, "••• Vous entrez dans le canal principal : Place.", false,false);
            }
            else
            {
                btnChannel.ButtonText = "Animations";
                btnChannel.Color1 = (Color)ColorConverter.ConvertFromString("#FF149A2D");
                btnChannel.Color2 = (Color)ColorConverter.ConvertFromString("#FF005613");
                chat.OnColoredMessage(Colors.DarkViolet, "••• Vous entrez dans le canal animation & tournoi : Animations.", false, false);
            }
            _animationChat = !_animationChat;
            btnChannel.Update();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnDiscord.ClickedAnimation();
            System.Diagnostics.Process.Start("https://discordapp.com/invite/seEZAwV");
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            btnDiscord.ReleasedAnimation();
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            btnNote.ClickedAnimation();
            Notes note = new Notes(_admin.Client.NotesAdmin);
            note.Show();
        }

        private void Image_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            btnNote.ReleasedAnimation();
        }

        private void btnFAQ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forum.battlecityalpha.xyz/thread-681.html");
        }

        private void btnCGU_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forum.battlecityalpha.xyz/thread-20.html");
        }

        private void tbChat_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    NetworkData data = ParseMessage(tbChat.GetText());
                    if (data != null)
                        _admin.Client.Send(data.Type, data.Packet);
                    tbChat.Clear();
                    break;
            }
            e.Handled = true;
        }

        private NetworkData ParseMessage(string txt)
        {
            if (txt[0] == '/')
            {
                txt = txt.Substring(1);
                string cmd = txt.Split(' ')[0].ToString().ToUpper();
                switch (cmd)
                {
                    case "ANIM":
                        return new NetworkData(PacketType.ChatMessage, _cmdParser.AnimationMessage(txt.Substring(cmd.Length + 1)));
                    case "INFO":
                        return new NetworkData(PacketType.ChatMessage, _cmdParser.InformationMessage(txt.Substring(cmd.Length + 1)));
                    case "SETMOTD":
                        return new NetworkData(PacketType.ChatMessage, _cmdParser.SetMessageOfTheDay(txt.Substring(cmd.Length + 1)));
                    case "SETGREET":
                        return new NetworkData(PacketType.ChatMessage, _cmdParser.SetGreet(txt.Substring(cmd.Length + 1)));
                    default:
                        _admin_ChatMessage(FormExecution.AppDesignConfig.LauncherMessageColor, "••• Cette commande n'existe pas.", false, false);
                        return null;
                }
            }

            return new NetworkData(PacketType.ChatMessage, _cmdParser.StandardMessage(txt));
        }
    }
}
