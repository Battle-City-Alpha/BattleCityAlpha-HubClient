using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using hub_client.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class AnimationsScheduleAdministrator
    {
        public GameClient Client;
        private AnimationsSchedule _window;

        public AnimationsScheduleAdministrator(GameClient client)
        {
            Client = client;
            Client.LoadAnimations += Client_LoadAnimations;
        }

        private void Client_LoadAnimations(Dictionary<string, string> colors, Animation[] animations)
        {
            if (_window == null)
            {
                _window = new AnimationsSchedule(this, animations, colors);
                _window.Show();
            }
            else
                _window.LoadAnims(animations);
        }

        public void SendAnimationUpdate(Animation anim, bool remove)
        {
            Client.Send(PacketType.UpdateAnimation, new StandardClientUpdateAnimation
            {
                Animation = anim,
                Remove = remove
            });
        }
    }
}
