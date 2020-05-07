using BCA.Common;
using BCA.Network.Packets.Enums;
using System.Collections.Generic;
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

    public class PlayerItemNameComparer : IComparer<PlayerItem>
    {
        public int Compare(PlayerItem x, PlayerItem y)
        {
            if (x.Rank != y.Rank)
                return y.Rank.CompareTo(x.Rank);
            return x.Username.CompareTo(y.Username);
        }
    }
}
