﻿using BCA.Common;
using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class ToolsAdministrator
    {
        public GameClient Client;

        public event Action<Customization[]> LoadAvatars;

        public ToolsAdministrator(GameClient client)
        {
            Client = client;

            Client.LoadAvatars += Client_LoadAvatars;
        }

        private void Client_LoadAvatars(Customization[] avatars)
        {
            LoadAvatars?.Invoke(avatars);
        }
    }
}
