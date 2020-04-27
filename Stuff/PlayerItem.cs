using BCA.Common;
using BCA.Network.Packets.Enums;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Stuff
{
    public class PlayerItem : PlayerInfo
    {
        public SolidColorBrush ChatColor { get; set; }
        public bool IsOnDuel => this.State == PlayerState.Duel;
        public BitmapImage AvatarPic { get; set; }
    }
}
