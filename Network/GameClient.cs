using BCA.Common;
using BCA.Common.Enums;
using BCA.Network;
using BCA.Network.Packets;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromServer;
using hub_client.Helpers;
using hub_client.WindowsAdministrator;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace hub_client.Network
{
    public class GameClient : BinaryClient
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private string _username;
        private string _password;

        public event Action<string, string, bool> PopMessageBox;
        public event Action<PlayerInfo, DuelType> ChoicePopBox;
        public event Action<string> LaunchYGOPro;
        #region ChatForm Events
        public event Action<Color, string, bool, bool> ChatMessageRecieved;
        public event Action<PlayerInfo> AddHubPlayer;
        public event Action<PlayerInfo> RemoveHubPlayer;
        public event Action Shutdown;
        public event Action<string, string> ClearChat;
        public event Action<string[]> Banlist;
        #endregion
        #region RegisterForm Events
        public event Action RegistrationComplete;
        #endregion
        #region LoginForm Events
        public event Action LoginComplete;
        #endregion
        #region PrivateForm Events
        public event Action<string, string> PrivateMessageReceived;
        #endregion
        #region ProfilForm Events
        public event Action<StandardServerProfilInfo> ProfilUpdate;
        #endregion
        #region "Arena Events"
        public event Action<Room> UpdateRoom;
        #endregion
        #region Shop Events
        public event Action<int, int, int, int, int> UpdateBoosterInfo;
        #endregion
        #region PurchaseForm Events
        public event Action<int[]> PurchaseItem;
        #endregion
        #region PanelForm Events
        public event Action<PlayerInfo[]> UpdatePanelUserlist;
        public event Action<string[], string, string, int> UpdatePanelUser;
        #endregion

        #region Administrator
        public ChatAdministrator ChatAdmin;
        public RegisterAdminstrator RegisterAdmin;
        public LoginAdminstrator LoginAdmin;
        public NotesAdministrator NotesAdmin;
        public ProfilAdministrator ProfilAdmin;
        public ArenaAdministrator ArenaAdmin;
        public ShopAdministrator ShopAdmin;
        public PurchaseAdministrator PurchaseAdmin;
        public PanelAdministrator PanelAdmin;
        #endregion

        public PlayerManager PlayerManager;

        public GameClient() : base(new NetworkClient())
        {
            PacketReceived += GameClient_PacketReceived;
            Disconnected += Client_Disconnected;
            InitAdministrator();
            InitManager();
        }

        private void InitAdministrator()
        {
            ChatAdmin = new ChatAdministrator(this);
            RegisterAdmin = new RegisterAdminstrator(this);
            LoginAdmin = new LoginAdminstrator(this);
            NotesAdmin = new NotesAdministrator(this);
            ProfilAdmin = new ProfilAdministrator(this);
            ArenaAdmin = new ArenaAdministrator(this);
            ShopAdmin = new ShopAdministrator(this);
            PurchaseAdmin = new PurchaseAdministrator(this);
            PanelAdmin = new PanelAdministrator(this);
        }

        private void InitManager()
        {
            PlayerManager = new PlayerManager();
        }

        public void OpenPopBox(string text, string title, bool showdialog = false)
        {
            logger.Trace("Open PopBox - Text : {0}, Title : {1}", text, title);
            Application.Current.Dispatcher.Invoke(() => PopMessageBox?.Invoke(text, title, showdialog));
        }

        public string Username()
        {
            return _username;
        }

        public void InitPlayer(string username, string password)
        {
            _username = username;
            _password = password;
        }

        private void Client_Disconnected(Exception ex)
        {
            logger.Fatal(ex);
            OpenPopBox("Vous avez été déconnecté du serveur.", "Problème");
            Thread.Sleep(1500);
            Shutdown?.Invoke();
        }


        public PlayerInfo GetPlayerInfo(string username)
        {
            return PlayerManager.GetInfos(username);
        }

        public void Send(PacketType packetId, Packet packet)
        {
            logger.Trace("PACKET SEND - {0} : {1}", packetId, packet);
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write((short)packetId);
                    writer.Write(JsonConvert.SerializeObject(packet));
                }
                Send(stream.ToArray());
            }
        }

        public void Send(NetworkData data)
        {
            Send(data.Type, data.Packet);
        }

        private void GameClient_PacketReceived(BinaryReader reader)
        {
            PacketType packetType = (PacketType)reader.ReadInt16();
            string packet = reader.ReadString();

            logger.Trace("PACKET RECEIVED - {0} : {1}", packetType, packet);

            switch (packetType)
            {
                case PacketType.ChatMessage:
                    OnChatMessage(JsonConvert.DeserializeObject<StandardServerChatMessage>(packet));
                    break;
                case PacketType.Register:
                    OnRegister(JsonConvert.DeserializeObject<StandardServerRegister>(packet));
                    break;
                case PacketType.Login:
                    OnLogin(JsonConvert.DeserializeObject<StandardServerLogin>(packet));
                    break;
                case PacketType.AddHubPlayer:
                    OnAddHubPlayer(JsonConvert.DeserializeObject<StandardServerAddHubPlayer>(packet));
                    break;
                case PacketType.RemoveHubPlayer:
                    OnRemoveHubPlayer(JsonConvert.DeserializeObject<StandardServerRemoveHubPlayer>(packet));
                    break;
                case PacketType.PlayerList:
                    OnUpdateHubPlayerList(JsonConvert.DeserializeObject<StandardServerPlayerlist>(packet));
                    break;
                case PacketType.CommandError:
                    OnCommandError(JsonConvert.DeserializeObject<StandardServerCommandError>(packet));
                    break;
                case PacketType.Kick:
                    OnKick(JsonConvert.DeserializeObject<StandardServerKick>(packet));
                    break;
                case PacketType.Ban:
                    OnBan(JsonConvert.DeserializeObject<StandardServerBan>(packet));
                    break;
                case PacketType.Mute:
                    OnMute(JsonConvert.DeserializeObject<StandardServerMute>(packet));
                    break;
                case PacketType.PrivateMessage:
                    OnPrivateMessage(JsonConvert.DeserializeObject<StandardServerPrivateMessage>(packet));
                    break;
                case PacketType.Profil:
                    OnProfilUpdate(JsonConvert.DeserializeObject<StandardServerProfilInfo>(packet));
                    break;
                case PacketType.UpdateCollection:
                    OnUpdateCollection(JsonConvert.DeserializeObject<StandardServerUpdateCollection>(packet));
                    break;
                case PacketType.DuelRequest:
                    OnDuelRequest(JsonConvert.DeserializeObject<StandardServerDuelRequest>(packet));
                    break;
                case PacketType.UpdateRoom:
                    OnUpdateRoom(JsonConvert.DeserializeObject<StandardServerUpdateRoom>(packet));
                    break;
                case PacketType.AskBooster:
                    OnAskBooster(JsonConvert.DeserializeObject<StandardServerAskBooster>(packet));
                    break;
                case PacketType.PurchaseItem:
                    OnPurchaseItem(JsonConvert.DeserializeObject<StandardServerPurchaseItem>(packet));
                    break;
                case PacketType.Clear:
                    OnClearChat(JsonConvert.DeserializeObject<StandardServerClear>(packet));
                    break;
                case PacketType.PanelUpdate:
                    OnPanelUpdateProfile(JsonConvert.DeserializeObject<StandardServerPanelUpdateProfile>(packet));
                    break;
                case PacketType.PanelUserlist:
                    OnPanelUserlist(JsonConvert.DeserializeObject<StandardServerPanelUserlist>(packet));
                    break;
                case PacketType.Banlist:
                    OnBanlist(JsonConvert.DeserializeObject<StandardServerBanlist>(packet));
                    break;
            }
        }

        private void OnChatMessage(StandardServerChatMessage packet)
        {
            ChatMessageType type = packet.Type;

            Color c;
            string msg;
            bool italic = false;
            bool bold = false;

            switch (type)
            {
                case ChatMessageType.Standard:
                    c = FormExecution.AppDesignConfig.StandardMessageColor;
                    msg = ParseUsername(packet.Player.Username, packet.Player.Rank) + ":" + packet.Message;
                    break;
                case ChatMessageType.Animation:
                    c = FormExecution.AppDesignConfig.AnimationMessageColor;
                    msg = "[Animation - " + packet.Player.Username + "]:" + packet.Message;
                    break;
                case ChatMessageType.Information:
                    c = FormExecution.AppDesignConfig.InformationMessageColor;
                    msg = "[Information - " + packet.Player.Username + "]:" + packet.Message;
                    bold = true;
                    break;
                case ChatMessageType.Greet:
                    c = FormExecution.AppDesignConfig.GreetMessageColor;
                    msg = "[Greet - " + packet.Player.Username + "]:" + packet.Message;
                    break;
                case ChatMessageType.Staff:
                    c = FormExecution.AppDesignConfig.StaffMessageColor;
                    msg = "[Staff - " + packet.Player.Username + "]:" + packet.Message;
                    italic = true;
                    break;
                default:
                    c = FormExecution.AppDesignConfig.LauncherMessageColor;
                    msg = "••• Une erreur s'est produite.";
                    break;
            }

            ChatMessageRecieved?.Invoke(c, msg, bold, italic);
            logger.Trace("CHAT MESSAGE - Type : {0} | Player : {1} | Message : {2}", packet.Type, packet.Player, packet.Message);
        }

        private void OnRegister(StandardServerRegister packet)
        {
            if (!packet.Success)
            {
                switch (packet.Reason)
                {
                    case RegisterFailReason.EmailAlreadyUsed:
                        OpenPopBox("L'adresse email est déja utilisée.", "Problème");
                        break;
                    case RegisterFailReason.InvalidEmail:
                        OpenPopBox("L'adresse email est invalide.", "Problème");
                        break;
                    case RegisterFailReason.InvalidUsername:
                        OpenPopBox("Le nom d'utilisateur contient des caractères spéciaux interdits.", "Problème");
                        break;
                    case RegisterFailReason.UnknownProblem:
                        OpenPopBox("Erreur inconnue rencontrée.", "Problème");
                        break;
                    case RegisterFailReason.UsernameAlreadyUsed:
                        OpenPopBox("Le nom d'utilisateur est déja utilisée.", "Problème");
                        break;
                    case RegisterFailReason.UsernameTooLong:
                        OpenPopBox("Le nom d'utilisateur est trop long, il doit être inférieur à 20 caractères.", "Problème");
                        break;
                }
            }
            else
            {
                OpenPopBox("Votre inscription s'est bien déroulée. L'équipe de Battle City Alpha vous souhaite bon jeu et vous remercie d'avoir choisi notre plateforme.", "Bienvenue !");
                Application.Current.Dispatcher.Invoke(() => RegistrationComplete?.Invoke());
            }

            logger.Trace("REGISTER - Success : {0}, Reason : {1}", packet.Success, packet.Reason);
        }

        private void OnLogin(StandardServerLogin packet)
        {
            if (!packet.Success)
            {
                switch (packet.Reason)
                {
                    case LoginFailReason.Banned:
                        OpenPopBox("Vous êtes banni.", "Problème");
                        break;
                    case LoginFailReason.InvalidCombinaison:
                        OpenPopBox("La combinaison utilisateur/mot de passe est invalide.", "Problème");
                        break;
                    case LoginFailReason.UsernameDoesntExist:
                        OpenPopBox("Le nom d'utilisateur n'existe pas.", "Problème");
                        break;
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => LoginComplete?.Invoke());
            }

            logger.Trace("Login - Success : {0}, Reason : {1}", packet.Success, packet.Reason);
        }

        private void OnAddHubPlayer(StandardServerAddHubPlayer packet)
        {
            PlayerManager.UpdatePlayer(packet.Infos);
            AddHubPlayer?.Invoke(packet.Infos);
            logger.Trace("AddHubPlayer - {0}", packet.Infos);
        }

        private void OnRemoveHubPlayer(StandardServerRemoveHubPlayer packet)
        {
            RemoveHubPlayer?.Invoke(packet.Infos);
            PlayerManager.Remove(packet.Infos);
            logger.Trace("RemoveHubPlayer - {0}", packet.Infos);
        }

        private void OnUpdateHubPlayerList(StandardServerPlayerlist packet)
        {
            foreach (PlayerInfo infos in packet.Userlist)
            {
                PlayerManager.UpdatePlayer(infos);
                AddHubPlayer?.Invoke(infos);
            }
            logger.Trace("UpdatePlayerList - {0}", packet.Userlist);
        }

        private void OnCommandError(StandardServerCommandError packet)
        {
            CommandErrorType type = packet.Type;

            Color c = FormExecution.AppDesignConfig.LauncherMessageColor;
            string msg;
            bool italic = false;
            bool bold = false;

            switch (type)
            {
                case CommandErrorType.ArgTooLong:
                    msg = "••• Un des arguments donné est trop long.";
                    break;
                case CommandErrorType.NoError:
                    msg = "••• L'opération a réussie.";
                    break;
                case CommandErrorType.NotVip:
                    msg = "••• Seul les joueurs VIP peuvent effectuer cette opération.";
                    break;
                case CommandErrorType.SmallRank:
                    msg = "••• Vous ne disposez pas de droits suffisants pour effectuer cette opération.";
                    break;
                case CommandErrorType.UnknownError:
                    msg = "••• Une erreur inconnue s'est produite durant cette opération.";
                    break;
                case CommandErrorType.PlayerNotConnected:
                    msg = "••• L'utilisateur ciblé n'est pas connecté.";
                    break;
                case CommandErrorType.NotEnoughMoney:
                    msg = "Tu n'as pas assez de points !";
                    break;
                default:
                    msg = "••• Erreur inconnue, impossible à traiter.";
                    break;
            }
            if (packet.MessageBox)
                OpenPopBox(msg, "Erreur");
            else
                ChatMessageRecieved?.Invoke(c, msg, italic, bold);
            logger.Trace("COMMAND ERROR MESSAGE MESSAGE - Type : {0} |  Message : {1}", packet.Type, msg);
        }

        public void OnKick(StandardServerKick packet)
        {
            OpenPopBox("Vous avez été kické par : " + packet.Kicker + " pour la raison : " + packet.Reason, "Ejection du serveur");
            logger.Trace("KICKED - By : {0} | Reason : {1}", packet.Kicker,  packet.Reason);
            Shutdown?.Invoke();
        }

        public void OnBan(StandardServerBan packet)
        {
            OpenPopBox("Vous avez été banni par : " + packet.Banner + " pour la raison : " + packet.Reason + " pour une durée de " + packet.Time, "Banni du serveur");
            logger.Trace("BANNED - By : {0} | Time : {1} | Reason : {2}", packet.Banner, packet.Time, packet.Reason);
            Shutdown?.Invoke();
        }

        public void OnMute(StandardServerMute packet)
        {
            OpenPopBox("Vous avez été rendu muet par : " + packet.Muter + " pour la raison : " + packet.Reason + " pour une durée de " + packet.Time, "Mute");
            logger.Trace("MUTED - By : {0} | Time : {1} | Reason : {2}", packet.Muter, packet.Time, packet.Reason);
        }

        public void OnPrivateMessage(StandardServerPrivateMessage packet)
        {
            packet.Message = ParseUsername(packet.Player.Username, packet.Player.Rank) + ":" + packet.Message;
            PrivateMessageReceived?.Invoke(packet.Player.Username, packet.Message);
            logger.Trace("PRIVATE MESSAGE RECEIVED - From : {0} | Message : {1}", packet.Player, packet.Message);
        }

        public void OnProfilUpdate(StandardServerProfilInfo packet)
        {
            ProfilUpdate?.Invoke(packet);
            logger.Trace("PROFIL INFO - Target : {0} |  Infos : {1}", packet.Username, packet);
        }

        public void OnUpdateCollection(StandardServerUpdateCollection packet)
        {
            PlayerManager.UpdateCollection(packet.Collection);

            string arg = "-j";
            
            switch(packet.Reason)
            {
                case AskCollectionReason.Deck_Edit:
                    arg = "-d";
                    break;
                case AskCollectionReason.Duel:
                    arg = "-j";
                    break;
            }
            LaunchYGOPro?.Invoke(arg);
            logger.Trace("UPDATE COLLECTION -  Collection : {0} | Reason : {1}", packet.Collection, packet.Reason);
        }

        public void OnDuelRequest(StandardServerDuelRequest packet)
        {
            logger.Trace("DUEL REQUEST - From {0} | Type : {1}", packet.Player.Username, packet.Type);
            Application.Current.Dispatcher.Invoke(() => ChoicePopBox?.Invoke(packet.Player, packet.Type));
        }

        public void OnUpdateRoom(StandardServerUpdateRoom packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateRoom?.Invoke(packet.Room));
            logger.Trace("UPDATE ROOM - Id : {0} | Type : {1} | Players : {2}", packet.Room.Id, packet.Room.Type, packet.Room.Players);
        }

        public void OnAskBooster(StandardServerAskBooster packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateBoosterInfo?.Invoke(packet.CardGot, packet.TotalCard, packet.Price, packet.CardPerPack, packet.BattlePoints));
            logger.Trace("UPDATE BOOSTER INFO - {0}/{1} | Price : {2} | BP : {3}", packet.CardGot, packet.TotalCard, packet.Price, packet.BattlePoints);
        }

        public void OnPurchaseItem(StandardServerPurchaseItem packet)
        {
            Application.Current.Dispatcher.Invoke(() => PurchaseItem?.Invoke(packet.Cards));
            logger.Trace("PURCHASE ITEMS - {0}", packet.Cards);
        }

        public void OnClearChat(StandardServerClear packet)
        {
            Application.Current.Dispatcher.Invoke(() => ClearChat?.Invoke(packet.Username, packet.Reason));
            logger.Trace("CLEAR CHAT - Username : {0} | Reason : {1}", packet.Username, packet.Reason);
        }

        public void OnPanelUserlist(StandardServerPanelUserlist packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePanelUserlist?.Invoke(packet.Players));
            logger.Trace("UPDATE PANEL USERLIST - Players : {0}", packet.Players);
        }

        public void OnPanelUpdateProfile(StandardServerPanelUpdateProfile packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePanelUser?.Invoke(packet.Accounts, packet.IP, packet.Observation, packet.Points));
            logger.Trace("UPDATE PANEL PLAYER - Players : {0} | IP : {1} | Observation : {2} | Points : {3}", packet.Accounts, packet.IP, packet.Observation, packet.Points);
        }

        public void OnBanlist(StandardServerBanlist packet)
        {
            Application.Current.Dispatcher.Invoke(() => Banlist?.Invoke(packet.Players));
            logger.Trace("BANLIST - {0}", packet.Players);
        }

        public string ParseUsername(string username, PlayerRank rank)
        {
            switch (rank)
            {
                case PlayerRank.Owner:
                    return "#Administrateur#" + username;
                case PlayerRank.Moderator:
                    return "#Modérateur#" + username;
                case PlayerRank.Animator:
                    return "#Animateur#" + username;
                case PlayerRank.Developper:
                    return "#Développeur#" + username;
                case PlayerRank.Contributor:
                    return "#Contributeur#" + username;
                default:
                    return username;
            }
        }

    }
}