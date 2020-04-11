using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Client.LoadTitles += Client_LoadTitles; ;
        }

        private void Client_LoadTitles(Dictionary<int, string> titles)
        {
            _titles = titles;
            LoadTitles?.Invoke(GlobalTools.GetDictionnaryValues(titles));
        }

        public void ChangeTitle(string title)
        {
            int id = _titles.FirstOrDefault(x => x.Value == title).Key;
            StandardClientChangeTitle packet = new StandardClientChangeTitle { Id = Convert.ToInt32(id) };
            FormExecution.Client.Send(PacketType.ChangeTitle, packet);
        }
    }
}
