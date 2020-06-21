using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using System;

namespace hub_client.WindowsAdministrator
{
    public class PrestigeShopAdministrator
    {
        public GameClient Client;

        public event Action<int> UpdatePrestigePoints;
        public event Action<int> UpdateProgress;
        public event Action<CustomSpecialPack> UpdatePack;

        public PrestigeShopAdministrator(GameClient client)
        {
            Client = client;

            Client.UpdatePP += Client_UpdatePP;
            Client.UpdateProgress += Client_UpdateProgress;
            Client.UpdateSpecialPack += Client_UpdateSpecialPack;
        }

        private void Client_UpdateSpecialPack(CustomSpecialPack pack)
        {
            UpdatePack?.Invoke(pack);
        }

        private void Client_UpdateProgress(int progress)
        {
            UpdateProgress?.Invoke(progress);
        }

        private void Client_UpdatePP(int pp)
        {
            UpdatePrestigePoints?.Invoke(pp);
        }

        public void SendAskMonthPack()
        {
            Client.Send(PacketType.AskMonthPack, new StandardClientAskMonthPack { });
        }
        public void SendBuyResetStats()
        {
            Client.Send(PacketType.ResetStats, new StandardClientResetStats { });
        }
        public void SendBuyVIP()
        {
            Client.Send(PacketType.BuyVIP, new StandardClientBuyVIP { });
        }
        public void SendBuyDoubleBP()
        {
            Client.Send(PacketType.BuyDoubleBP, new StandardClientDoubleBP { });
        }
        public void SendBuyInfiniGreet()
        {
            Client.Send(PacketType.BuyGreet, new StandardClientGreetInfinite { });
        }
        public void SendBuyChangeUsername(string username)
        {
            Client.Send(PacketType.ChangeUsername, new StandardClientChangeUsername
            {
                Username = username
            });
        }
        public void SendBuyUsernameColor(string color)
        {
            Client.Send(PacketType.UsernameColor, new StandardClientUsernameColor
            {
                ChatColor = color
            });
        }
        public void SendCustomCustomization(string url, CustomizationType ctype)
        {
            Client.Send(PacketType.BuyOwnCustom, new StandardClientBuyOwnCustomization
            {
                URL = url,
                CustomType = ctype
            });
        }
        public void SendAskPrestigeCustomizations(CustomizationType ctype)
        {
            Client.Send(PacketType.AskPrestigeCustoms, new StandardClientAskPrestigeCustomizations { Ctype = ctype });
            if (ctype == CustomizationType.Avatar || ctype == CustomizationType.Sleeve || ctype == CustomizationType.Partner)
                FormExecution.OpenPrestigeCustomizationsViewer();
            else if (ctype == CustomizationType.Title)
                FormExecution.OpenPrestigeTitleViewer();
            else if (ctype == CustomizationType.Border)
                FormExecution.OpenPrestigeCustomizationsVerticalViewer();
        }
        public void SendBuySpecialPack(int id)
        {
            Client.Send(PacketType.BuySpecialPack, new StandardClientBuySpecialPack
            {
                Id = id
            });
        }
    }
}
