using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using BCA.Network.Packets.Standard.FromServer;
using hub_client.Network;
using hub_client.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace hub_client.WindowsAdministrator
{
    public class ProfilAdministrator
    {
        public GameClient Client;

        public event Action<int, string, int, int, int, int, int, int, PlayerRank, int, int, int, int> UpdateProfil;

        public ProfilAdministrator(GameClient client)
        {
            Client = client;
            Client.ProfilUpdate += Client_ProfilUpdate;
        }

        private void Client_ProfilUpdate(StandardServerProfilInfo infos)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateProfil?.Invoke(infos.AvatarId, infos.Username, infos.CardNumber, infos.Level, infos.Exp, infos.RankedWin, infos.RankedLose, infos.ELO, infos.Rank, infos.UnrankedWin, infos.UnrankedLose, infos.GiveUp, infos.RageQuit));
        }

        public void OpenAvatarsForm()
        {
            Client.Send(PacketType.LoadAvatar, new StandardClientLoadAvatars());
            AvatarsHandle form = new AvatarsHandle(Client.AvatarsHandleAdmin);
            form.Show();
        }
    }
}
