using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls.Controls_Stuff
{
    public class RoomResultItem
    {
        public string Opponent { get; set; }
        public BitmapImage AvatarPic { get; set; }
        public BitmapImage ResultPic { get; set; }
        public int ELO { get; set; }
        public int CurrentRank { get; set; }
        public int TotalRank { get; set; }
        public string GameDate { get; set; }
        public SolidColorBrush ELOColor => ELO > 0 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
    }
}
