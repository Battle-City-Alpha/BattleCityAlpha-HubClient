using BCA.Common;
using BCA.Network.Packets.Enums;
using System.Windows.Media;

namespace hub_client.Stuff
{
    public class PlayerItem : PlayerInfo
    {
        public SolidColorBrush ChatColor { get; set; }
        public bool IsOnDuel => this.State == PlayerState.Duel;
    }
}
