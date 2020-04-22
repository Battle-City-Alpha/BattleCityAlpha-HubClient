using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using System;

namespace hub_client.WindowsAdministrator
{
    public class AvatarsHandleAdministrator
    {
        public GameClient Client;

        public event Action<Customization[]> LoadAvatars;
        public AvatarsHandleAdministrator(GameClient client)
        {
            Client = client;
            Client.LoadAvatars += Client_LoadAvatars;
        }

        private void Client_LoadAvatars(Customization[] avatars)
        {
            LoadAvatars?.Invoke(avatars);
        }

        public void ChangeAvatar(int id)
        {
            StandardClientChangeAvatar packet = new StandardClientChangeAvatar { Id = id };
            Client.Send(PacketType.ChangeAvatar, packet);
        }
    }
}