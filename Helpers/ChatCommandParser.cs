using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Helpers
{
    public class ChatCommandParser
    {
        public StandardClientChatMessage StandardMessage(string txt)
        {
            return new StandardClientChatMessage
            {
                Type = ChatMessageType.Standard,
                Message = txt
            };
        }
        public StandardClientChatMessage AnimationMessage(string txt)
        {
            return new StandardClientChatMessage
            {
                Type = ChatMessageType.Animation,
                Message = txt
            };
        }

        public StandardClientChatMessage InformationMessage(string txt)
        {
            return new StandardClientChatMessage
            {
                Type = ChatMessageType.Information,
                Message = txt
            };
        }

        public StandardClientChatMessage SetMessageOfTheDay(string txt)
        {
            return new StandardClientChatMessage
            {
                Type = ChatMessageType.MOTD,
                Message = txt
            };
        }

        public StandardClientChatMessage SetGreet(string txt)
        {
            return new StandardClientChatMessage
            {
                Type = ChatMessageType.Greet,
                Message = txt
            };
        }

        public StandardClientKick Kick(string txt)
        {
            string[] args = txt.Split(' ');

            string target = args[0];
            PlayerInfo infos = FormExecution.Client.GetPlayerInfo(target);
            if (infos == null)
                return null;

            string reason;
            if (args.Length < 2)
                reason = "Aucune.";
            else
                reason = txt.Substring(target.Length + 1);
            return new StandardClientKick
            {
                Target = infos,
                Reason = reason
            };
        }

        public StandardClientBan Ban(string txt)
        {
            string[] args = txt.Split(' ');

            string target = args[0];
            PlayerInfo infos = FormExecution.Client.GetPlayerInfo(target);
            if (infos == null)
                return null;

            int time = -1;
            Int32.TryParse(args[1], out time);
            if (time == -1)
                return null;

            string reason;
            if (args.Length < 3)
                reason = "Aucune.";
            else
                reason = txt.Substring(args[0].Length + args[1].Length + 2);
            return new StandardClientBan
            {
                Target = infos,
                Time = time,
                Reason = reason
            };
        }

        public StandardClientMute Mute(string txt)
        {
            string[] args = txt.Split(' ');

            string target = args[0];
            PlayerInfo infos = FormExecution.Client.GetPlayerInfo(target);
            if (infos == null)
                return null;

            int time = -1;
            Int32.TryParse(args[1], out time);
            if (time == -1)
                return null;

            string reason;
            if (args.Length < 3)
                reason = "Aucune.";
            else
                reason = txt.Substring(args[0].Length + args[1].Length + 2);
            return new StandardClientMute
            {
                Target = infos,
                Time = time,
                Reason = reason
            };
        }

        public StandardClientClear ClearChat(string txt)
        {
            return new StandardClientClear
            {
                Reason = txt
            };
        }

        public StandardClientMPAll MPAll(string txt)
        {
            return new StandardClientMPAll
            {
                Message = txt
            };
        }
    }
}
