using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using BCA.Network.Packets.Standard.FromServer;
using hub_client.Network;
using hub_client.Windows;
using System;
using System.Windows;

namespace hub_client.WindowsAdministrator
{
    public class ProfilAdministrator
    {
        public GameClient Client;

        public event Action<StandardServerProfilInfo> UpdateProfil;

        public ProfilAdministrator(GameClient client)
        {
            Client = client;
            Client.ProfilUpdate += Client_ProfilUpdate;
        }

        private void Client_ProfilUpdate(StandardServerProfilInfo infos)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateProfil?.Invoke(infos));
        }

        public void OpenAvatarsForm(Profil window)
        {
            Client.Send(PacketType.LoadAvatar, new StandardClientLoadAvatars());
            /*AvatarsHandle form = new AvatarsHandle(Client.AvatarsHandleAdmin);
            form.Show();*/
            PrestigeCustomizationsViewerHorizontal viewer = new PrestigeCustomizationsViewerHorizontal(Client.PrestigeCustomizationsViewerAdmin, false);
            viewer.Owner = window;
            viewer.Show();
        }
        public void OpenTitlesForm(Profil window)
        {
            Client.Send(PacketType.AskTitle, new StandardClientAskTitles());
            TitlesHandle form = new TitlesHandle(Client.TitlesHandleAdmin, false);
            form.Owner = window;
            form.Show();
        }
        public void OpenBordersForm(Profil window)
        {
            Client.Send(PacketType.LoadBorders, new StandardClientLoadBorders());
            /*BordersHandle form = new BordersHandle(Client.BordersHandleAdmin);
            form.Show();*/

            PrestigeCustomizationViewerVertical viewer = new PrestigeCustomizationViewerVertical(Client.PrestigeCustomizationsViewerAdmin, false);
            viewer.Owner = window;
            viewer.Show();
        }
        public void OpenSleevesForm(Profil window)
        {
            Client.Send(PacketType.LoadSleeves, new StandardClientLoadSleeves());
            /*SleevesHandle form = new SleevesHandle(Client.SleevesHandleAdmin);
            form.Show();*/

            PrestigeCustomizationsViewerHorizontal viewer = new PrestigeCustomizationsViewerHorizontal(Client.PrestigeCustomizationsViewerAdmin, false);
            viewer.Owner = window;
            viewer.Show();
        }
    }
}
