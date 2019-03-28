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
            {
                FormExecution.Client?.OpenPopBox("Le joueur " + target + " n'est pas connecté.", "Erreur");
                return null;
            }

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
            {
                FormExecution.Client?.OpenPopBox("Le joueur " + target + " n'est pas connecté.", "Erreur");
                return null;
            }

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
            {
                FormExecution.Client?.OpenPopBox("Le joueur " + target + " n'est pas connecté.", "Erreur");
                return null;
            }

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

        public StandardClientGivePoints GiveBattlePoints(string txt)
        {
            string[] args = txt.Split(' ');

            string target = args[0];

            int points = -1;
            Int32.TryParse(args[1], out points);
            if (points == -1)
                return null;

            return new StandardClientGivePoints
            {
                Target = target,
                Points = points,
                PrestigePoint = false
            };
        }
        public StandardClientGivePoints GivePrestigePoints(string txt)
        {
            string[] args = txt.Split(' ');

            string target = args[0];

            int points = -1;
            Int32.TryParse(args[1], out points);
            if (points == -1)
                return null;

            return new StandardClientGivePoints
            {
                Target = target,
                Points = points,
                PrestigePoint = true
            };
        }
        public StandardClientGiveAvatar GiveAvatar(string txt)
        {
            string[] args = txt.Split(' ');
            string target = args[0];
            PlayerInfo infos = FormExecution.Client.GetPlayerInfo(target);
            if (infos == null)
            {
                FormExecution.Client?.OpenPopBox("Le joueur " + target + " n'est pas connecté.", "Erreur");
                return null;
            }

            int id = -1;
            Int32.TryParse(args[1], out id);
            if (id == -1)
                return null;

            return new StandardClientGiveAvatar
            {
                Target = target,
                Avatar = id
            };
        }
        public StandardClientGiveCard GiveCard(string txt)
        {
            string[] args = txt.Split(' ');
            string target = args[0];
            PlayerInfo infos = FormExecution.Client.GetPlayerInfo(target);
            if (infos == null)
            {
                FormExecution.Client?.OpenPopBox("Le joueur " + target + " n'est pas connecté.", "Erreur");
                return null;
            }

            int id = -1;
            Int32.TryParse(args[1], out id);
            if (id == -1)
                return null;

            return new StandardClientGiveCard
            {
                Target = target,
                Id = id
            };
        }

        public StandardClientEnabled EnabledAccount(string txt)
        {
            string[] args = txt.Split(' ');

            string target = args[0];

            string reason;
            if (args.Length < 2)
                reason = "Aucune.";
            else
                reason = txt.Substring(target.Length + 1);
            return new StandardClientEnabled
            {
                Target = target,
                Reason = reason
            };
        }

        public StandardClientDisabled DisabledAccount(string txt)
        {
            string[] args = txt.Split(' ');

            string target = args[0];

            string reason;
            if (args.Length < 2)
                reason = "Aucune.";
            else
                reason = txt.Substring(target.Length + 1);
            return new StandardClientDisabled
            {
                Target = target,
                Reason = reason
            };
        }

        public StandardClientRanker Ranker(string txt)
        {
            string[] args = txt.Split(' ');

            string target = args[0];
            PlayerInfo infos = FormExecution.Client.GetPlayerInfo(target);
            if (infos == null)
            {
                FormExecution.Client?.OpenPopBox("Le joueur " + target + " n'est pas connecté.", "Erreur");
                return null;
            }

            int rang = -1;
            Int32.TryParse(args[1], out rang);
            if (rang == -1)
                return null;

            return new StandardClientRanker
            {
                Player = infos,
                Rank = (PlayerRank)rang
            };
        }
    }
}
