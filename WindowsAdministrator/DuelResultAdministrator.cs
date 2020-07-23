using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using hub_client.Windows;
using System;
using System.Windows;

namespace hub_client.WindowsAdministrator
{
    public class DuelResultAdministrator
    {
        public GameClient Client;

        public event Action<bool> RevengeAnswer;

        public DuelResultAdministrator(GameClient client)
        {
            Client = client;
            Client.LaunchDuelResultBox += Client_LaunchDuelResultBox;
            Client.DuelResultAnswer += Client_DuelResultAnswer;
        }

        private void Client_DuelResultAnswer(bool result)
        {
            RevengeAnswer?.Invoke(result);
        }

        private void Client_LaunchDuelResultBox(int bps, int exps, bool win, int opponent, RoomConfig config, int roomID)
        {
            DuelResult box = new DuelResult(this, bps, exps, win, opponent, config, roomID);
            box.Topmost = true;
            box.Show();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => box.Activate()));
        }

        public void SendDuelResultAnswer(bool result, RoomConfig config, int opponent, int roomID)
        {
            Client.Send(PacketType.DuelResultAnswer, new StandardClientDuelResultAnswer
            {
                Config = config,
                Opponent = opponent,
                Result = result,
                RoomID = roomID
            });
        }
    }
}
