using BCA.Common;
using BCA.Common.Bets;
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
        public bool IsShadowDuel { get; set;  }
        public string CaptionText { get; set; }
        public BitmapImage Image { get { return FormExecution.AssetsManager.GetImage("Duel", Type.ToString()); } }
        public SolidColorBrush RoomColor { get; set; }
        public SolidColorBrush CaptionColor => new SolidColorBrush(FormExecution.AppDesignConfig.GetGameColor("CaptionDuelTextColor"));
        public Bet Bet { get; set; }
        public int ObserversCount { get; set; }
        public bool HasObservers => ObserversCount > 0;
    }
}
