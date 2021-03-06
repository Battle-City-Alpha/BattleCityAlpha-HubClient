﻿using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using hub_client.Windows;
using System;
using System.Collections.Generic;
using System.Windows;

namespace hub_client.WindowsAdministrator
{
    public class AnimationsScheduleAdministrator
    {
        public GameClient Client;
        private AnimationsSchedule _window;

        public int AnimationOffset;

        public AnimationsScheduleAdministrator(GameClient client)
        {
            Client = client;
            Client.LoadAnimations += Client_LoadAnimations;

            AnimationOffset = 0;
        }

        private void Client_LoadAnimations(Dictionary<string, string> colors, Animation[] animations)
        {
            if (_window == null || !_window.IsVisible)
            {
                _window = new AnimationsSchedule(this, animations, colors);
                _window.Show();
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _window.Activate()));
            }
            else
                _window.LoadAnims(animations);
        }

        public void SendAnimationUpdate(Animation anim, bool remove)
        {
            Client.Send(PacketType.UpdateAnimation, new StandardClientUpdateAnimation
            {
                Animation = anim,
                Offset = AnimationOffset,
                Remove = remove
            });
        }
        public void SendAskAnimations()
        {
            Client.Send(PacketType.AskAnimations, new StandardClientAskAnimations
            {
                Offset = AnimationOffset
            });
        }
    }
}
