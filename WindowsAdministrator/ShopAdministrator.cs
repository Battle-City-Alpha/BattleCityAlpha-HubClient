using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using System;

namespace hub_client.WindowsAdministrator
{
    public class ShopAdministrator
    {
        public GameClient Client;

        public event Action<int, int, int, int, int> UpdateBoosterInfo;
        public event Action<int> UpdateBattlePoints;

        public ShopAdministrator(GameClient client)
        {
            Client = client;
            client.UpdateBoosterInfo += Client_UpdateBoosterInfo;
            client.UpdateBattlePoints += Client_UpdateBattlePoints;
        }

        private void Client_UpdateBattlePoints(int points)
        {
            UpdateBattlePoints?.Invoke(points);
        }

        private void Client_UpdateBoosterInfo(int cardgot, int totalcard, int price, int cardperpack, int bp)
        {
            UpdateBoosterInfo?.Invoke(cardgot, totalcard, price, cardperpack, bp);
        }

        public void AskBooster(string tag)
        {
            Client.Send(PacketType.AskBooster, new StandardClientAskBooster { PurchaseTag = tag });
        }
        public void Purchase(string tag, int numberPacket)
        {
            Client.Send(PacketType.PurchaseItem, new StandardClientPurchase { Tag = tag, NumberPacket = numberPacket });
        }
        public void AskBoosterCollection(string tag)
        {
            Client.Send(PacketType.AskBoosterCollection, new StandardClientAskBoosterCollection { PurchaseTag = tag });
        }
    }
}
