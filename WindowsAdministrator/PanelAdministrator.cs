using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using System;

namespace hub_client.WindowsAdministrator
{
    public class PanelAdministrator
    {
        public GameClient Client;

        public event Action<PlayerInfo[]> UpdatePlayersList;
        public event Action<string[], string, string, int> UpdateProfile;

        public PanelAdministrator(GameClient client)
        {
            Client = client;

            client.UpdatePanelUser += Client_UpdatePanelUser;
            client.UpdatePanelUserlist += Client_UpdatePanelUserlist;
        }

        private void Client_UpdatePanelUserlist(PlayerInfo[] players)
        {
            UpdatePlayersList?.Invoke(players);
        }

        private void Client_UpdatePanelUser(string[] accounts, string ip, string obs, int bp)
        {
            UpdateProfile?.Invoke(accounts, ip, obs, bp);
        }

        public void SendPanelUserlist()
        {
            Client.Send(PacketType.PanelUserlist, new StandardClientOpenPanel { });
        }
        public void PanelKick(string reason, PlayerInfo target)
        {
            Client.Send(PacketType.Kick, new StandardClientKick { Reason = reason, Target = target });
        }
        public void PanelMute(string reason, PlayerInfo target, int time)
        {
            Client.Send(PacketType.Mute, new StandardClientMute { Reason = reason, Target = target, Time = time });
        }
        public void PanelBan(string reason, PlayerInfo target, int time)
        {
            Client.Send(PacketType.Ban, new StandardClientBan { Reason = reason, Target = target, Time = time });
        }
        public void PanelAskProfile(PlayerInfo target)
        {
            Client.Send(PacketType.PanelAskProfile, new StandardClientAskPanelProfile { Player = target });
        }
        public void PanelUpdate(PlayerInfo target, string obs)
        {
            Client.Send(PacketType.PanelUpdate, new StandardClientPanelProfileUpdate { Player = target, Observation = obs });
        }
    }
}