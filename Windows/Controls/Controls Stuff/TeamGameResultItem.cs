using System.Windows.Media.Imaging;

namespace hub_client.Windows.Controls.Controls_Stuff
{
    public class TeamGameResultItem
    {
        public string Looser { get; set; }
        public BitmapImage LooserAvatar { get; set; }
        public BitmapImage LooserTeam { get; set; }
        public string Winner { get; set; }
        public BitmapImage WinnerAvatar { get; set; }
        public BitmapImage WinnerTeam { get; set; }
    }
}
