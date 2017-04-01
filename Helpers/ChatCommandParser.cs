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
    }
}
