using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Helpers;
using hub_client.Network;
using hub_client.Windows;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace hub_client.WindowsAdministrator
{
    public class ChatAdministrator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public GameClient Client;

        private ChatCommandParser _cmdParser;

        public event Action LoginComplete;
        public event Action<Color, string, bool, bool> SpecialChatMessage;
        public event Action<Color, PlayerInfo, string> PlayerChatMessage;
        public event Action ShowSmileys;
        public event Action<PlayerInfo, bool> AddHubPlayer;
        public event Action<PlayerInfo, bool> RemoveHubPlayer;
        public event Action<PlayerInfo> UpdateHubPlayer;
        public event Action<PlayerInfo[], PlayerState> UpdateHubPlayers;
        public event Action<string, string> ClearChat;
        public event Action<PlayerInfo, string, bool> Mutechat;
        public event Action DailyQuestNotification;
        public event Action<bool> AnimationNotification;

        public ChatAdministrator(GameClient client)
        {
            Client = client;
            Client.PlayerChatMessageRecieved += Client_PlayerChatMessageRecieved;
            Client.SpecialChatMessageRecieved += Client_SpecialChatMessageRecieved;
            Client.LoginComplete += Client_LoginComplete;
            Client.AddHubPlayer += Client_AddHubPlayer;
            Client.RemoveHubPlayer += Client_RemoveHubPlayer;
            Client.UpdateHubPlayer += Client_UpdateHubPlayer;
            Client.UpdateHubPlayers += Client_UpdateHubPlayers;
            Client.ClearChat += Client_ClearChat;
            Client.Banlist += Client_Banlist;
            Client.Mutelist += Client_Mutelist;
            Client.DailyQuestNotification += Client_DailyQuestNotification;
            Client.AnimationNotification += Client_AnimationNotification;
            Client.Mutechat += Client_Mutechat;
            Client.GetRoomsList += Client_GetRoomsList;

            _cmdParser = new ChatCommandParser();
        }

        private void Client_GetRoomsList(Dictionary<RoomState, Room[]> rooms)
        {
            Color c = FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor");
            Client_SpecialChatMessageRecieved(c, "Duel en attente :", false, false);
            foreach (Room r in rooms[RoomState.Waiting])
                Client_SpecialChatMessageRecieved(c, string.Format("[{0}] - {1} - {2}", r.Id, r.Config.Type, string.Join<PlayerInfo>(" vs ", r.Players)), false, false);
            Client_SpecialChatMessageRecieved(c, "Duel en cours :", false, false);
            foreach (Room r in rooms[RoomState.Dueling])
                Client_SpecialChatMessageRecieved(c, string.Format("[{0}] - {1} - {2}", r.Id, r.Config.Type, string.Join<PlayerInfo>(" vs ", r.Players)), false, false);
        }

        private void Client_Mutechat(PlayerInfo sender, string reason, bool ismuted)
        {
            Mutechat?.Invoke(sender, reason, ismuted);
        }

        private void Client_AnimationNotification(bool update)
        {
            AnimationNotification?.Invoke(update);
        }

        private void Client_DailyQuestNotification()
        {
            DailyQuestNotification?.Invoke();
        }

        private void Client_UpdateHubPlayer(PlayerInfo player)
        {
            UpdateHubPlayer?.Invoke(player);
        }

        private void Client_UpdateHubPlayers(PlayerInfo[] players, PlayerState state)
        {
            UpdateHubPlayers?.Invoke(players, state);
        }

        private void Client_SpecialChatMessageRecieved(Color c, string text, bool isBold, bool isItalic)
        {
            SpecialChatMessage?.Invoke(c, text, isBold, isItalic);
        }

        private void Client_PlayerChatMessageRecieved(Color c, PlayerInfo p, string msg)
        {
            PlayerChatMessage?.Invoke(c, p, msg);
        }

        private void Client_Banlist(string[] players)
        {
            string bl = "Banlist : ";
            foreach (string player in players)
                bl += player + ",";

            SpecialChatMessage?.Invoke(FormExecution.AppDesignConfig.GetGameColor("StaffMessageColor"), bl, false, false);
        }
        private void Client_Mutelist(string[] players)
        {
            string bl = "Mutelist : ";
            foreach (string player in players)
                bl += player + ",";

            SpecialChatMessage?.Invoke(FormExecution.AppDesignConfig.GetGameColor("StaffMessageColor"), bl, false, false);
        }

        private void Client_ClearChat(string username, string reason)
        {
            ClearChat?.Invoke(username, reason);
        }

        private void Client_RemoveHubPlayer(PlayerInfo infos)
        {
            RemoveHubPlayer?.Invoke(infos, true);
        }

        private void Client_AddHubPlayer(PlayerInfo infos, bool showmessage)
        {
            AddHubPlayer?.Invoke(infos, showmessage);
        }

        private void Client_LoginComplete()
        {
            LoginComplete?.Invoke();
        }

        public void SendMessage(string msg)
        {
            NetworkData data = ParseMessage(msg);
            if (data != null && data.Packet != null)
            {
                Client.Send(data);
                logger.Info("Chat send : {0}.", data);
            }
        }
        public void SendProfileAsking()
        {
            NetworkData data = new NetworkData(PacketType.Profil, new StandardClientProfilAsk { UserID = FormExecution.PlayerInfos.UserId });
            Client.Send(data);
        }
        public void SendDeck()
        {
            NetworkData data = new NetworkData(PacketType.UpdateCollection, new StandardClientAskCollection());
            Client.Send(data);
        }
        public void SendTradeRequest(PlayerInfo target)
        {
            Client.Send(PacketType.TradeRequest, new StandardClientTradeRequest { Player = target });
        }
        public void SendSpectateRequest(PlayerInfo target)
        {
            Client.Send(PacketType.SpectatePlayer, new StandardClientSpectate { Target = target });
        }
        public void AddBlacklistPlayer(PlayerInfo target)
        {
            Client.BlacklistManager.AddPlayer(target);
            Client.BlacklistManager.Save();
        }
        public void RemoveBlacklistPlayer(PlayerInfo target)
        {
            Client.BlacklistManager.RemovePlayer(target);
            Client.BlacklistManager.Save();
        }
        public void SendDonationBP(string amount, PlayerInfo target)
        {
            int pts;
            if (int.TryParse(amount, out pts))
            {
                Client.Send(PacketType.DonPoints, new StandardClientDonPoints
                {
                    Target = target,
                    Amount = pts
                });
            }
            else
                SpecialChatMessage?.Invoke(FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor"), "Vous n'avez pas indiqué un nombre valable de BPs.", false, false);
        }
        public void AskSelectCard(AskCollectionReason reason)
        {
            Client.Send(PacketType.AskSelectCard, new StandardClientAskSelectCard
            {
                Reason = reason
            });
        }
        public void SendAskProfil(PlayerInfo infos)
        {
            Client.Send(PacketType.Profil, new StandardClientProfilAsk
            {
                UserID = infos.UserId
            });
        }
        public void SendShareDeck(PlayerInfo infos, string[] deckfile, string deckname)
        {
            Client.Send(PacketType.ShareDeck, new StandardClientSendDeck
            {
                Target = infos,
                Deckfile = deckfile,
                Deckname = deckname
            });
        }
        public void SendShareReplay(PlayerInfo infos, byte[] replayfile, string replayname)
        {
            Client.Send(PacketType.ShareReplay, new StandardClientShareReplay
            {
                Target = infos,
                ReplayFile = replayfile,
                ReplayName = replayname
            });
        }

        public void SendGiveCard(Dictionary<int, PlayerCard> cards, PlayerInfo target)
        {
            Client.Send(PacketType.CardDonation, new StandardClientCardDonation
            {
                Target = target,
                Cards = cards
            });
        }

        private NetworkData ParseMessage(string txt)
        {
            try
            {
                if (txt[0] == '/')
                {
                    txt = txt.Substring(1);
                    string cmd = txt.Split(' ')[0].ToString().ToUpper();
                    switch (cmd)
                    {
                        case "ANIM":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.AnimationMessage(txt.Substring(cmd.Length + 1)));
                        case "INFO":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.InformationMessage(txt.Substring(cmd.Length + 1)));
                        case "SETMOTD":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.SetMessageOfTheDay(txt.Substring(cmd.Length + 1)));
                        case "SETGREET":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.SetGreet(txt.Substring(cmd.Length + 1)));
                        case "KICK":
                            return new NetworkData(PacketType.Kick, _cmdParser.Kick(txt.Substring(cmd.Length + 1)));
                        case "BAN":
                            return new NetworkData(PacketType.Ban, _cmdParser.Ban(txt.Substring(cmd.Length + 1)));
                        case "UNBAN":
                            return new NetworkData(PacketType.Unban, _cmdParser.Unban(txt.Substring(cmd.Length + 1)));
                        case "MUTE":
                            return new NetworkData(PacketType.Mute, _cmdParser.Mute(txt.Substring(cmd.Length + 1)));
                        case "UNMUTE":
                            return new NetworkData(PacketType.Unmute, _cmdParser.Unmute(txt.Substring(cmd.Length + 1)));
                        case "CLEAR":
                            return new NetworkData(PacketType.Clear, _cmdParser.ClearChat(txt.Length > cmd.Length ? txt.Substring(cmd.Length + 1) : "."));
                        case "MUTECHAT":
                            return new NetworkData(PacketType.Mutechat, _cmdParser.MuteChat(txt.Length > cmd.Length ? txt.Substring(cmd.Length + 1) : "."));
                        case "MPALL":
                            return new NetworkData(PacketType.MPAll, _cmdParser.MPAll(txt.Substring(cmd.Length + 1)));
                        case "PANEL":
                            Panel panel = new Panel(Client.PanelAdmin);
                            panel.Show();
                            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => panel.Activate()));
                            return null;
                        case "BANLIST":
                            return new NetworkData(PacketType.Banlist, new StandardClientBanlist { });
                        case "MUTELIST":
                            return new NetworkData(PacketType.Mutelist, new StandardClientMutelist { });
                        case "HELP":
                            return new NetworkData(PacketType.Help, new StandardClientAskHelp { });
                        case "GIVEBATTLEPOINTS":
                            return new NetworkData(PacketType.GivePoints, _cmdParser.GiveBattlePoints(txt.Substring(cmd.Length + 1)));
                        case "GIVEPRESTIGEPOINTS":
                            return new NetworkData(PacketType.GivePoints, _cmdParser.GivePrestigePoints(txt.Substring(cmd.Length + 1)));
                        case "GIVECARD":
                            return new NetworkData(PacketType.GiveCard, _cmdParser.GiveCard(txt.Substring(cmd.Length + 1)));
                        case "GIVEAVATAR":
                            return new NetworkData(PacketType.GiveAvatar, _cmdParser.GiveAvatar(txt.Substring(cmd.Length + 1)));
                        case "GIVEBORDER":
                            return new NetworkData(PacketType.GiveBorder, _cmdParser.GiveBorder(txt.Substring(cmd.Length + 1)));
                        case "GIVESLEEVE":
                            return new NetworkData(PacketType.GiveSleeve, _cmdParser.GiveSleeve(txt.Substring(cmd.Length + 1)));
                        case "GIVEPARTNER":
                            return new NetworkData(PacketType.GivePartner, _cmdParser.GivePartner(txt.Substring(cmd.Length + 1)));
                        case "GIVETITLE":
                            return new NetworkData(PacketType.GiveTitle, _cmdParser.GiveTitle(txt.Substring(cmd.Length + 1)));
                        case "ENABLED":
                            return new NetworkData(PacketType.EnabledAccount, _cmdParser.EnabledAccount(txt.Substring(cmd.Length + 1)));
                        case "DISABLED":
                            return new NetworkData(PacketType.DisabledAccount, _cmdParser.DisabledAccount(txt.Substring(cmd.Length + 1)));
                        case "PROMOTE":
                            return new NetworkData(PacketType.Ranker, _cmdParser.Ranker(txt.Substring(cmd.Length + 1)));
                        case "MAINTENANCE":
                            return new NetworkData(PacketType.Maintenance, _cmdParser.AskMaintenance(txt.Substring(cmd.Length + 1)));
                        case "MAINTENANCESTOP":
                            return new NetworkData(PacketType.StopMaintenance, _cmdParser.StopMaintenance());
                        case "DUELSERVERSTOP":
                            return new NetworkData(PacketType.DuelServerStop, _cmdParser.DuelServerStop(txt.Substring(cmd.Length + 1)));
                        case "DUELSERVERRESTART":
                            return new NetworkData(PacketType.DuelServerRestart, _cmdParser.DuelServerRestart());
                        case "NEXTRANKINGSEASON":
                            return new NetworkData(PacketType.NextRankingSeason, _cmdParser.AskNextRankingSeason());
                        case "BLACKLIST":
                            Blacklist blacklist = new Blacklist(Client.BlacklistManager);
                            blacklist.Show();
                            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => blacklist.Activate()));
                            return null;
                        case "STATS":
                            SpecialChatMessage?.Invoke(FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor"), "••• Il y a " + FormExecution.GetChatWindow().Players.Count + " utilisateurs connectés.", false, false);
                            return null;
                        case "SMILEYS":
                            ShowSmileys?.Invoke();
                            return null;
                        case "LOGS":
                            _cmdParser.OpenLogFolder();
                            return null;
                        case "ROOMSLIST":
                            return new NetworkData(PacketType.RoomList, new StandardClientAskRoomList { });
                        case "KILLROOM":
                            return new NetworkData(PacketType.KillRoom, _cmdParser.KillRoom(txt.Substring(cmd.Length + 1)));
                        default:
                            SpecialChatMessage?.Invoke(FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor"), "••• Cette commande n'existe pas.", false, false);
                            return null;
                    }
                }
                return new NetworkData(PacketType.ChatMessage, _cmdParser.StandardMessage(txt));
            }
            catch (Exception ex)
            {
                SpecialChatMessage?.Invoke(FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor"), "••• Une erreur s'est produite.", false, false);
                logger.Error("Chat input error : {0}", ex.ToString());
                return null;
            }
        }

        public void SendAskDailyQuest()
        {
            Client.Send(PacketType.AskDailyQuest, new StandardClientAskDailyQuest { });
        }
        public void SendAskAnimations()
        {
            Client.Send(PacketType.AskAnimations, new StandardClientAskAnimations { Offset = Client.AnimationsScheduleAdmin.AnimationOffset });
        }
    }
}
