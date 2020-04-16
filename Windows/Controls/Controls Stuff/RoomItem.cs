using BCA.Common.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls.Controls_Stuff
{
    public class RoomItem
    {
        public int Id { get; set; }
        public string Players { get; set; }
        public RoomType Type { get; set; }
        public bool NeedPassword { get; set; }
        public bool IsRanked { get; set; }
        public BitmapImage Image { get { return FormExecution.AssetsManager.GetImage("Duel", Type.ToString()); } }
    }
}
