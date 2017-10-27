using hub_client.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class PurchaseAdministrator
    {
        public GameClient Client;
        public event Action<int[]> PurchaseItem;

        public PurchaseAdministrator(GameClient client)
        {
            Client = client;
            client.PurchaseItem += Client_PurchaseItem; ;
        }

        private void Client_PurchaseItem(int[] cards)
        {
            string path = Path.Combine(FormExecution.path, "BattleCityAlpha", "deck", "new_cards.ydk");
            if (File.Exists(path))
                File.Delete(path);
            string file = "#New Cards";
            foreach (int id in cards)
                file += Environment.NewLine + id.ToString();
            File.WriteAllText(path, file);

            PurchaseItem?.Invoke(cards);
        }
    }
}
