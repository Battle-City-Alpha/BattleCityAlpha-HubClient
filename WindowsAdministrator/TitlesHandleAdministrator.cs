using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace hub_client.WindowsAdministrator
{
    public class TitlesHandleAdministrator
    {
        public GameClient Client;
        private Dictionary<int, string> _titles;

        public event Action<List<string>> LoadTitles;
        public TitlesHandleAdministrator(GameClient client)
        {
            Client = client;
            Client.LoadTitles += Client_LoadTitles;
            Client.LoadPrestigeCustomizations += Client_LoadPrestigeCustomizations;
        }

        private void Client_LoadPrestigeCustomizations(Customization[] titles)
        {
            if (titles[0].CustomizationType != CustomizationType.Title)
                return;
            _titles = new Dictionary<int, string>();
            foreach (Customization c in titles)
                _titles.Add(c.Id, c.URL);
            LoadTitles?.Invoke(GlobalTools.GetDictionnaryValues(_titles));
        }

        private void Client_LoadTitles(Dictionary<int, string> titles)
        {
            _titles = titles;
            LoadTitles?.Invoke(GlobalTools.GetDictionnaryValues(_titles));
        }

        public void ChangeTitle(string title)
        {
            int id = _titles.FirstOrDefault(x => x.Value == title).Key;
            StandardClientChangeTitle packet = new StandardClientChangeTitle { Id = Convert.ToInt32(id) };
            FormExecution.Client.Send(PacketType.ChangeTitle, packet);
        }
        public void BuyPrestigeTitle(string title)
        {
            int id = _titles.FirstOrDefault(x => x.Value == title).Key;
            Client.Send(PacketType.BuyPrestigeCustom, new StandardClientBuyPrestigeCustomization
            {
                CustomType = CustomizationType.Title,
                Id = id
            });
        }
    }
}
