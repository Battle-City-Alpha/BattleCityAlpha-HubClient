using BCA.Common;
using BCA.Common.Enums;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls.Controls_Stuff
{
    public class RoomItem
    {
        public int Id { get; set; }
        public RoomConfig Config { get; set; }
        public string Players1 { get; set; }
        public string Players2 { get; set; }
        public RoomType Type { get; set; }
        public bool NeedPassword { get; set; }
        public bool IsRanked { get; set; }
        public BitmapImage Image { get { return FormExecution.AssetsManager.GetImage("Duel", Type.ToString()); } }
        public SolidColorBrush RoomColor => (Config.Banlist != 0 || (Config.StartDuelLP != 8000 && Type != RoomType.Tag) || (Config.StartDuelLP != 16000 && Type == RoomType.Tag) || Config.CardByHand != 5 || Config.MasterRules != 5 || Config.DrawCount != 1 | Config.NoShuffleDeck == true) ? new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CustomRoomColor")) : new SolidColorBrush(Colors.Black);
    }
}
