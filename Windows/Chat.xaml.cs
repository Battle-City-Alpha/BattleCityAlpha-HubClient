using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Configuration;
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

        public Chat(ChatAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;
            _admin.ChatMessage += _admin_ChatMessage;
            this.Loaded += Chat_Loaded;
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
            BottomButtons.AddRange(new[] { btnProfil, btnFAQ, btnCGU, btnNote, btnDiscord });

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

        private void _admin_ChatMessage(string msg)
        {
            //chat.AppendText(msg);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    //StandardClientChatMessage packet = new StandardClientChatMessage(chatTB.Text);
                    //FormExecution.Client.Send(PacketType.ChatMessage, packet);
                    //chatTB.Clear();
                    break;
            }
            e.Handled = true;
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
            Notes note = new Notes();
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
    }
}
