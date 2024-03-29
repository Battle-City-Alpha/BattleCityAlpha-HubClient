﻿using BCA.Common;
using BCA.Common.Bets;
using BCA.Common.Enums;
using BCA.Network;
using BCA.Network.Helpers;
using BCA.Network.Packets;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using BCA.Network.Packets.Standard.FromServer;
using BCA.Network.Packets.Story_Mode.FromServer;
using BCA.Story_Mode;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Helpers;
using hub_client.Stuff;
using hub_client.WindowsAdministrator;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace hub_client.Network
{
    public class GameClient : BinaryClient
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private string _username;

        public event Action<string, string> PopMessageBox;
        public event Action<string, string> PopMessageBoxShowDialog;
        public event Action<PlayerInfo, RoomConfig, ChoiceBoxType, string> ChoicePopBox;
        public event Action<PlayerInfo, RoomConfig, Bet> ShadowDuelRequest;
        public event Action<int, RoomType> RoomNeedPassword;
        public event Action<Room, string> LaunchYGOPro;
        public event Action<string> LaunchYGOProWithoutRoom;
        public event Action<Customization, Customization, Customization, Customization, int, string, int> LoadPlayerCustomizations;
        public event Action LaunchTrade;
        public event Action CloseBrocante;
        public event Action<int, int, bool, int, RoomConfig, int> LaunchDuelResultBox;
        public event Action<bool> DuelResultAnswer;
        public event Action<PlayerInfo, string[], string> RecieveDeck;
        public event Action<PlayerInfo, byte[], string> RecieveReplay;
        public event Action<CustomizationType, string, int> CustomizationAchievement;
        public event Action<string, List<int>, List<int>, List<CardRarity>> LoadBoosterCollection;
        public event Action<int, int, int, int> LoadMonthPack;
        public event Action<Dictionary<int, MonthlyBonus>, int, int[]> GetMonthlyBonus;
        public event Action<Room> RecieveRoomIsWaiting;
        #region Daily Quests
        public event Action<DailyQuestType[], string[], int[]> GetDailyQuests;
        public event Action<bool, DailyQuestType, int, bool> DailyQuestReward;
        public event Action<bool, DailyQuestType, string> ChangeDailyQuest;
        #endregion
        #region BonusBox Events
        public event Action<BonusType, int, string, int[]> LaunchBonusBox;
        #endregion
        #region  Events
        #endregion
        #region ChatForm Events
        public event Action<Color, string, bool, bool> SpecialChatMessageRecieved;
        public event Action<Color, PlayerInfo, string> PlayerChatMessageRecieved;
        public event Action<PlayerInfo, bool> AddHubPlayer;
        public event Action<PlayerInfo> RemoveHubPlayer;
        public event Action<PlayerInfo> UpdateHubPlayer;
        public event Action Shutdown;
        public event Action Restart;
        public event Action<string, string> ClearChat;
        public event Action<PlayerInfo, string, bool> Mutechat;
        public event Action<string[]> Banlist;
        public event Action<string[]> Mutelist;
        public event Action<PlayerInfo[], PlayerState> UpdateHubPlayers;
        public event Action DailyQuestNotification;
        public event Action<bool> AnimationNotification;
        public event Action<Dictionary<RoomState, Room[]>> GetRoomsList;
        #endregion
        #region RegisterForm Events
        public event Action RegistrationComplete;
        #endregion
        #region LoginForm Events
        public event Action LoginComplete;
        #endregion
        #region PrivateForm Events
        public event Action<PlayerInfo, string> PrivateMessageReceived;
        #endregion
        #region ProfilForm Events
        public event Action<StandardServerProfilInfo> ProfilUpdate;
        #endregion
        #region "Arena Events"
        public event Action<Room> UpdateRoom;
        #endregion
        #region Shop Events
        public event Action<int, int, int, int, int> UpdateBoosterInfo;
        public event Action<int> UpdateBattlePoints;
        #endregion
        #region PurchaseForm Events
        public event Action<int[]> PurchaseItem;
        #endregion
        #region PanelForm Events
        public event Action<PlayerInfo[]> UpdatePanelUserlist;
        public event Action<PlayerInfo, string[], string, string, int> UpdatePanelUser;
        #endregion
        #region TradeForm Events
        public event Action<int, PlayerInfo[], Dictionary<int, PlayerCard>[]> InitTrade;
        public event Action<PlayerInfo, string> GetMessage;
        public event Action<List<PlayerCard>> UpdateCardsToOffer;
        public event Action TradeExit;
        public event Action TradeEnd;
        #endregion
        #region AvatarsHandleForm Events
        public event Action<Customization[]> LoadAvatars;
        #endregion
        #region BordersHandleForm Events
        public event Action<Customization[]> LoadBorders;
        #endregion
        #region SleevesHandleForm Events
        public event Action<Customization[]> LoadSleeves;
        #endregion
        #region PartnerHandleForm Events
        public event Action<Customization[]> LoadPartners;
        #endregion
        #region TitlesHandleForm Events
        public event Action<Dictionary<int, string>> LoadTitles;
        #endregion
        #region BrocanteForm Events
        public event Action<List<BrocanteCard>> LoadBrocante;
        #endregion
        #region SelectCardForm Events
        public event Action<Dictionary<int, PlayerCard>, bool> LoadSelectCard;
        #endregion
        #region GiveCard Events
        public event Action<Dictionary<int, PlayerCard>> LoadGiveCards;
        #endregion
        #region OfflineMessagesBox Events
        public event Action<OfflineMessage[]> LoadOfflineMessages;
        #endregion
        #region PrestigeShopForm Events
        public event Action<int> UpdatePP;
        public event Action<int> UpdateProgress;
        public event Action<CustomSpecialPack> UpdateSpecialPack;
        public event Action<Customization[]> LoadPrestigeCustomizations;
        #endregion
        #region DataRetrievalWindow Events
        public event Action<bool, LoginFailReason, bool> DataRetrievalInfos;
        #endregion
        #region RankingDisplay Events
        public event Action<RankingPlayerInfos[], Customization[], int> ShowRanking;
        public event Action<RankingTeamInfos[], int> ShowTeamRanking;
        #endregion
        #region Games History Events
        public event Action<RoomResult[]> GetGamesHistory;
        #endregion
        #region Animations Schedule Events
        public event Action<Dictionary<string, string>, Animation[]> LoadAnimations;
        #endregion
        #region TeamProfile Events
        public event Action<StandardServerAskTeam> TeamProfileInfos;
        #endregion
        #region TeamGamesHistoryEvents
        public event Action<TeamGameResult[]> TeamGamesHistory;
        #endregion
        #region Story Mode Events
        public event Action<OpenWorldCharacter[]> GetOpenWorldCharacters;
        public event Action<SceneInfo[]> GetScenes;
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
        public TradeAdministrator TradeAdmin;
        public BrocanteAdministrator BrocanteAdmin;
        public ToolsAdministrator ToolsAdmin;
        public SelectCardAdministrator SelectCardAdmin;
        public AvatarsHandleAdministrator AvatarsHandleAdmin;
        public DuelRequestAdministrator DuelRequestAdmin;
        public TitlesHandleAdministrator TitlesHandleAdmin;
        public BordersHandleAdministrator BordersHandleAdmin;
        public SleevesHandleAdministrator SleevesHandleAdmin;
        public PrestigeShopAdministrator PrestigeShopAdmin;
        public PrestigeCustomizationsViewerAdministrator PrestigeCustomizationsViewerAdmin;
        public DataRetrievalAdministrator DataRetrievalAdmin;
        public RankingDisplayAdministrator RankingDisplayAdmin;
        public GiveCardAdministrator GiveCardAdmin;
        public GamesHistoryAdministrator GamesHistoryAdmin;
        public MonthPackViewerAdministrator MonthPackViewerAdmin;
        public DailyQuestAdministrator DailyQuestAdmin;
        public AnimationsScheduleAdministrator AnimationsScheduleAdmin;
        public DuelResultAdministrator DuelResultAdmin;
        public TeamProfileAdministrator TeamProfileAdmin;
        public TeamGamesHistoryAdministrator TeamGamesHistoryAdmin;
        public StoryModeConsoleAdministrator StoryModeConsoleAdmin;
        #endregion

        public PlayerManager PlayerManager;
        public BlacklistManager BlacklistManager;

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
            TradeAdmin = new TradeAdministrator(this);
            BrocanteAdmin = new BrocanteAdministrator(this);
            ToolsAdmin = new ToolsAdministrator(this);
            SelectCardAdmin = new SelectCardAdministrator(this);
            AvatarsHandleAdmin = new AvatarsHandleAdministrator(this);
            DuelRequestAdmin = new DuelRequestAdministrator(this);
            TitlesHandleAdmin = new TitlesHandleAdministrator(this);
            BordersHandleAdmin = new BordersHandleAdministrator(this);
            SleevesHandleAdmin = new SleevesHandleAdministrator(this);
            PrestigeShopAdmin = new PrestigeShopAdministrator(this);
            PrestigeCustomizationsViewerAdmin = new PrestigeCustomizationsViewerAdministrator(this);
            DataRetrievalAdmin = new DataRetrievalAdministrator(this);
            RankingDisplayAdmin = new RankingDisplayAdministrator(this);
            GiveCardAdmin = new GiveCardAdministrator(this);
            GamesHistoryAdmin = new GamesHistoryAdministrator(this);
            MonthPackViewerAdmin = new MonthPackViewerAdministrator(this);
            DailyQuestAdmin = new DailyQuestAdministrator(this);
            AnimationsScheduleAdmin = new AnimationsScheduleAdministrator(this);
            DuelResultAdmin = new DuelResultAdministrator(this);
            TeamProfileAdmin = new TeamProfileAdministrator(this);
            TeamGamesHistoryAdmin = new TeamGamesHistoryAdministrator(this);
            StoryModeConsoleAdmin = new StoryModeConsoleAdministrator(this);
        }

        private void InitManager()
        {
            PlayerManager = new PlayerManager();
            BlacklistManager = new BlacklistManager();
        }

        public void OpenPopBox(string text, string title)
        {
            logger.Trace("Open PopBox - Text : {0}, Title : {1}", text, title);
            Application.Current.Dispatcher.Invoke(() => PopMessageBox?.Invoke(text, title));
        }

        public string Username()
        {
            return _username;
        }

        public void InitPlayer(string username)
        {
            _username = username;
        }

        private void Client_Disconnected(Exception ex)
        {
            logger.Fatal(ex);
            //Thread.Sleep(1500);
            Application.Current.Dispatcher.Invoke(() => Restart?.Invoke());
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
                    byte[] toSend = CompressHelper.Zip(JsonConvert.SerializeObject(packet));
                    writer.Write(toSend.Length);
                    writer.Write(toSend);
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
            int size = reader.ReadInt32();
            string packet = CompressHelper.Unzip(reader.ReadBytes(size));

            logger.Trace("PACKET RECEIVED - {0} : {1}", packetType, packet);

            try
            {
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
                    case PacketType.UpdateHubPlayer:
                        OnUpdateHubPlayer(JsonConvert.DeserializeObject<StandardServerUpdateHubPlayer>(packet));
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
                    case PacketType.Muted:
                        OnMuted(JsonConvert.DeserializeObject<StandardServerMuted>(packet));
                        break;
                    case PacketType.Unmute:
                        OnUnmute(JsonConvert.DeserializeObject<StandardServerUnmute>(packet));
                        break;
                    case PacketType.DisabledAccount:
                        OnDisabled(JsonConvert.DeserializeObject<StandardServerDisabledAccount>(packet));
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
                    case PacketType.GivePoints:
                        OnGivePoints(JsonConvert.DeserializeObject<StandardServerGetPoints>(packet));
                        break;
                    case PacketType.GiveCard:
                        OnGiveCard(JsonConvert.DeserializeObject<StandardServerGetCard>(packet));
                        break;
                    case PacketType.CardDonation:
                        OnCardDonation(JsonConvert.DeserializeObject<StandardServerCardDonation>(packet));
                        break;
                    case PacketType.GiveAvatar:
                        OnGiveAvatar(JsonConvert.DeserializeObject<StandardServerGetAvatar>(packet));
                        break;
                    case PacketType.TradeRequest:
                        OnTradeRequest(JsonConvert.DeserializeObject<StandardServerTradeRequest>(packet));
                        break;
                    case PacketType.TradeRequestAnswer:
                        OnTradeRequestAnswer(JsonConvert.DeserializeObject<StandardServerTradeRequestAnswer>(packet));
                        break;
                    case PacketType.TradeMessage:
                        OnTradeMessage(JsonConvert.DeserializeObject<StandardServerTradeMessage>(packet));
                        break;
                    case PacketType.TradeProposition:
                        OnTradeProposition(JsonConvert.DeserializeObject<StandardServerTradeProposition>(packet));
                        break;
                    case PacketType.TradeExit:
                        OnTradeExit(JsonConvert.DeserializeObject<StandardServerTradeExit>(packet));
                        break;
                    case PacketType.TradeEnd:
                        OnTradeEnd(JsonConvert.DeserializeObject<StandardServerTradeExit>(packet));
                        break;
                    case PacketType.LoadAvatar:
                        OnLoadAvatars(JsonConvert.DeserializeObject<StandardServerLoadAvatars>(packet));
                        break;
                    case PacketType.LoadBorders:
                        OnLoadBorders(JsonConvert.DeserializeObject<StandardServerLoadBorders>(packet));
                        break;
                    case PacketType.LoadSleeves:
                        OnLoadSleeves(JsonConvert.DeserializeObject<StandardServerLoadSleeves>(packet));
                        break;
                    case PacketType.LoadBrocante:
                        OnLoadBrocante(JsonConvert.DeserializeObject<StandardServerLoadBrocante>(packet));
                        break;
                    case PacketType.AskSelectCard:
                        OnLoadSelectCard(JsonConvert.DeserializeObject<StandardServerLoadSelectCard>(packet));
                        break;
                    case PacketType.SearchCard:
                        OnSearchCard(JsonConvert.DeserializeObject<StandardServerSearchCard>(packet));
                        break;
                    case PacketType.CloseBrocante:
                        OnCloseBrocante(JsonConvert.DeserializeObject<StandardServerCloseBrocante>(packet));
                        break;
                    case PacketType.DuelRequest:
                        OnDuelRequest(JsonConvert.DeserializeObject<StandardServerDuelRequest>(packet));
                        break;
                    case PacketType.DuelRequestAnswer:
                        OnDuelRequestAnswer(JsonConvert.DeserializeObject<StandardServerDuelRequestResult>(packet));
                        break;
                    case PacketType.UpdateRoom:
                        OnUpdateRoom(JsonConvert.DeserializeObject<StandardServerUpdateRoom>(packet));
                        break;
                    case PacketType.DuelStart:
                        OnDuelStart(JsonConvert.DeserializeObject<StandardServerDuelStart>(packet));
                        break;
                    case PacketType.SpectatePlayer:
                        OnDuelSpectate(JsonConvert.DeserializeObject<StandardServerDuelSpectate>(packet));
                        break;
                    case PacketType.NeedRoomPassword:
                        OnRoomNeedPassword(JsonConvert.DeserializeObject<StandardServerNeedRoomPassword>(packet));
                        break;
                    case PacketType.GetBonus:
                        OnGetBonus(JsonConvert.DeserializeObject<StandardServerGetBonus>(packet));
                        break;
                    case PacketType.MonthlyBonus:
                        OnGetMonthlyBonus(JsonConvert.DeserializeObject<StandardServerGetMonthlyBonus>(packet));
                        break;
                    case PacketType.DuelResult:
                        OnDuelResult(JsonConvert.DeserializeObject<StandardServerDuelResult>(packet));
                        break;
                    case PacketType.GiveTitle:
                        OnGetTitle(JsonConvert.DeserializeObject<StandardServerGetTitle>(packet));
                        break;
                    case PacketType.GiveBorder:
                        OnGiveBorder(JsonConvert.DeserializeObject<StandardServerGetBorder>(packet));
                        break;
                    case PacketType.GiveSleeve:
                        OnGiveSleeve(JsonConvert.DeserializeObject<StandardServerGetSleeve>(packet));
                        break;
                    case PacketType.AskTitle:
                        OnLoadTitles(JsonConvert.DeserializeObject<StandardServerLoadTitles>(packet));
                        break;
                    case PacketType.Maintenance:
                        OnMaintenance(JsonConvert.DeserializeObject<StandardServerMaintenance>(packet));
                        break;
                    case PacketType.OfflineMessages:
                        OnOfflineMessages(JsonConvert.DeserializeObject<StandardServerOfflineMessages>(packet));
                        break;
                    case PacketType.LoadPlayerCustomization:
                        OnLoadPlayerCustomizationTextures(JsonConvert.DeserializeObject<StandardServerLoadPlayerCustomizationTextures>(packet));
                        break;
                    case PacketType.ResetStats:
                        OnResetStat(JsonConvert.DeserializeObject<StandardServerResetStat>(packet));
                        break;
                    case PacketType.ChangeUsername:
                        OnChangeUsername(JsonConvert.DeserializeObject<StandardServerChangeUsername>(packet));
                        break;
                    case PacketType.UsernameColor:
                        OnChangeChatColor(JsonConvert.DeserializeObject<StandardServerUsernameColor>(packet));
                        break;
                    case PacketType.BuyVIP:
                        OnBuyVIP(JsonConvert.DeserializeObject<StandardServerBuyVIP>(packet));
                        break;
                    case PacketType.BuyDoubleBP:
                        OnBuyDoubleBP(JsonConvert.DeserializeObject<StandardServerDoubleBP>(packet));
                        break;
                    case PacketType.BuyOwnCustom:
                        OnBuyOwnCustomization(JsonConvert.DeserializeObject<StandardServerBuyOwnCustomization>(packet));
                        break;
                    case PacketType.BuyPrestigeCustom:
                        OnBuyPrestigeCustomization(JsonConvert.DeserializeObject<StandardServerBuyPrestigeCustomization>(packet));
                        break;
                    case PacketType.BuyGreet:
                        OnBuyInfiniteGreet(JsonConvert.DeserializeObject<StandardServerGreetInfinite>(packet));
                        break;
                    case PacketType.BuyMonthPack:
                        OnBuyMonthPack(JsonConvert.DeserializeObject<StandardServerBuyMonthPack>(packet));
                        break;
                    case PacketType.BuySpecialPack:
                        OnBuySpecialPack(JsonConvert.DeserializeObject<StandardServerBuySpecialPack>(packet));
                        break;
                    case PacketType.OpenPrestigeShop:
                        OnOpenPrestigeShop(JsonConvert.DeserializeObject<StandardServerOpenPrestigeShop>(packet));
                        break;
                    case PacketType.GetCustomizationAchievement:
                        OnCustomizationAchievement(JsonConvert.DeserializeObject<StandardServerGetCustomizationAchievement>(packet));
                        break;
                    case PacketType.AskPrestigeCustoms:
                        OnLoadPrestigeCustomizations(JsonConvert.DeserializeObject<StandardServerLoadPrestigeCustomizations>(packet));
                        break;
                    case PacketType.DataRetrieval:
                        OnDataRetrievalInfos(JsonConvert.DeserializeObject<StandardServerDataRetrieval>(packet));
                        break;
                    case PacketType.NextRankingSeason:
                        OnNextRankingSeason(JsonConvert.DeserializeObject<StandardServerNextRankingSeason>(packet));
                        break;
                    case PacketType.ShareDeck:
                        OnShareDeck(JsonConvert.DeserializeObject<StandardServerSendDeck>(packet));
                        break;
                    case PacketType.ShareReplay:
                        OnShareReplay(JsonConvert.DeserializeObject<StandardServerShareReplay>(packet));
                        break;
                    case PacketType.GetRanking:
                        OnGetRanking(JsonConvert.DeserializeObject<StandardServerGetRanking>(packet));
                        break;
                    case PacketType.Ping:
                        OnPing(JsonConvert.DeserializeObject<StandardServerPing>(packet));
                        break;
                    case PacketType.AskGamesHistory:
                        OnGetGamesHistory(JsonConvert.DeserializeObject<StandardServerGamesHistory>(packet));
                        break;
                    case PacketType.AskBoosterCollection:
                        OnAskBoosterCollection(JsonConvert.DeserializeObject<StandardServerBoosterCollection>(packet));
                        break;
                    case PacketType.AskMonthPack:
                        OnAskMonthPack(JsonConvert.DeserializeObject<StandardServerAskMonthPack>(packet));
                        break;
                    case PacketType.AskDailyQuest:
                        OnGetDailyQuests(JsonConvert.DeserializeObject<StandardServerSendDailyQuests>(packet));
                        break;
                    case PacketType.GetDailyQuest:
                        OnDailyQuestReward(JsonConvert.DeserializeObject<StandardServerGetDailyQuestReward>(packet));
                        break;
                    case PacketType.ChangeDailyQuest:
                        OnChangeDailyQuest(JsonConvert.DeserializeObject<StandardServerChangeDailyQuest>(packet));
                        break;
                    case PacketType.DailyQuestNotification:
                        OnDailyQuestNotification(JsonConvert.DeserializeObject<StandardServerDailyQuestNotification>(packet));
                        break;
                    case PacketType.DuelServerStop:
                        OnDuelServerStop(JsonConvert.DeserializeObject<StandardServerDuelServerStop>(packet));
                        break;
                    case PacketType.AskAnimations:
                        OnGetAnimations(JsonConvert.DeserializeObject<StandardServerGetAnimations>(packet));
                        break;
                    case PacketType.AnimationNotification:
                        OnAnimationNotification(JsonConvert.DeserializeObject<StandardServerAnimationNotification>(packet));
                        break;
                    case PacketType.GivePartner:
                        OnGivePartner(JsonConvert.DeserializeObject<StandardServerGetPartner>(packet));
                        break;
                    case PacketType.LoadPartner:
                        OnLoadPartners(JsonConvert.DeserializeObject<StandardServerLoadPartners>(packet));
                        break;
                    case PacketType.WaitingRoom:
                        OnRecieveRoomIsWaiting(JsonConvert.DeserializeObject<StandardServerWaitingRoom>(packet));
                        break;
                    case PacketType.ShadowDuelRequest:
                        OnShadowDuelRequest(JsonConvert.DeserializeObject<StandardServerShadowDuelRequest>(packet));
                        break;
                    case PacketType.ShadowDuelRequestAnswer:
                        OnShadowDuelRequestAnswer(JsonConvert.DeserializeObject<StandardServerShadowDuelRequestResult>(packet));
                        break;
                    case PacketType.DuelResultAnswer:
                        OnDuelResultAnswer(JsonConvert.DeserializeObject<StandardServerDuelResultAnswer>(packet));
                        break;
                    case PacketType.Mutelist:
                        OnMutelist(JsonConvert.DeserializeObject<StandardServerMutelist>(packet));
                        break;
                    case PacketType.Mutechat:
                        OnMuteChat(JsonConvert.DeserializeObject<StandardServerMutechat>(packet));
                        break;
                    case PacketType.RoomList:
                        OnGetRoomsList(JsonConvert.DeserializeObject<StandardServerRoomList>(packet));
                        break;
                    case PacketType.CreateTeam:
                        OnBuyCreateTeam(JsonConvert.DeserializeObject<StandardServerCreateTeam>(packet));
                        break;
                    case PacketType.AskTeamProfile:
                        OnTeamProfil(JsonConvert.DeserializeObject<StandardServerAskTeam>(packet));
                        break;
                    case PacketType.GetTeamsRanking:
                        OnTeamsRanking(JsonConvert.DeserializeObject<StandardServerGetTeamsRanking>(packet));
                        break;
                    case PacketType.GetTeamGamesHistory:
                    case PacketType.GetTeamMemberGamesHistory:
                        OnTeamsGameHistory(JsonConvert.DeserializeObject<StandardServerTeamGamesHistory>(packet));
                        break;
                    case PacketType.GetOpenWorldCharacters:
                        OnGetOpenWorldCharacters(JsonConvert.DeserializeObject<StoryModeServerGetOpenWorldCharacters>(packet));
                        break;
                    case PacketType.GetScenes:
                        OnGetScenes(JsonConvert.DeserializeObject<StoryModeServerGetScenes>(packet));
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        private void OnChatMessage(StandardServerChatMessage packet)
        {
            if (BlacklistManager.CheckBlacklist(packet.Player))
                return;

            ChatMessageType type = packet.Type;

            Color c;
            string msg = "";
            bool italic = false;
            bool bold = false;

            switch (type)
            {
                case ChatMessageType.Standard:
                    c = FormExecution.AppDesignConfig.GetGameColor("StandardMessageColor");
                    break;
                case ChatMessageType.Animation:
                    c = FormExecution.AppDesignConfig.GetGameColor("AnimationMessageColor");
                    msg = "[Animation - " + packet.Player.Username + "]: " + packet.Message;
                    break;
                case ChatMessageType.Information:
                    c = FormExecution.AppDesignConfig.GetGameColor("InformationMessageColor");
                    msg = "**[Information - " + packet.Player.Username + "]:" + packet.Message + "**";
                    bold = true;
                    break;
                case ChatMessageType.Greet:
                    if (!FormExecution.ClientConfig.Greet)
                        return;
                    c = FormExecution.AppDesignConfig.GetGameColor("GreetMessageColor");
                    msg = "[Greet - " + packet.Player.Username + "]: " + packet.Message;
                    break;
                case ChatMessageType.Staff:
                    c = FormExecution.AppDesignConfig.GetGameColor("StaffMessageColor");
                    msg = "[Staff - " + packet.Player.Username + "]: " + packet.Message;
                    italic = true;
                    break;
                default:
                    c = FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor");
                    msg = "••• Une erreur s'est produite.";
                    break;
            }
            if (type != ChatMessageType.Standard)
                Application.Current.Dispatcher.Invoke(() => SpecialChatMessageRecieved?.Invoke(c, msg, bold, italic));
            else
                Application.Current.Dispatcher.Invoke(() => PlayerChatMessageRecieved?.Invoke(c, packet.Player, packet.Message));
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
                        OpenPopBox(string.Format("Vous êtes banni jusqu'au {0}. Raison : {1}.", packet.EndSanction, packet.SanctionReason), "Problème");
                        break;
                    case LoginFailReason.InvalidCombinaison:
                        OpenPopBox("La combinaison utilisateur/mot de passe est invalide.", "Problème");
                        break;
                    case LoginFailReason.UsernameDoesntExist:
                        OpenPopBox("Le nom d'utilisateur n'existe pas.", "Problème");
                        break;
                    case LoginFailReason.DisabledAccount:
                        OpenPopBox(string.Format("Votre compte est désactivé. Raison : {0}", packet.SanctionReason), "Problème");
                        break;
                    case LoginFailReason.UserAlreadyConnected:
                        OpenPopBox("Quelqu'un est déja connecté sur votre compte. Tentez de vous reconnecter maintenant.", "Problème");
                        break;
                    case LoginFailReason.Maintenance:
                        OpenPopBox("Une maintenance est en cours." + Environment.NewLine + "Raison: " + packet.MaintenanceReason + Environment.NewLine + "Temps estimé: " + packet.MaintenanceTimeEstimation.ToString() + "h.", "Maintenance");
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
            Application.Current.Dispatcher.Invoke(() => AddHubPlayer?.Invoke(packet.Infos, true));
            logger.Trace("AddHubPlayer - {0}", packet.Infos);
        }
        private void OnRemoveHubPlayer(StandardServerRemoveHubPlayer packet)
        {
            Application.Current.Dispatcher.Invoke(() => RemoveHubPlayer?.Invoke(packet.Infos));
            PlayerManager.Remove(packet.Infos);
            logger.Trace("RemoveHubPlayer - {0}", packet.Infos);
        }
        private void OnUpdateHubPlayer(StandardServerUpdateHubPlayer packet)
        {
            PlayerManager.UpdatePlayer(packet.Player);
            Application.Current.Dispatcher.Invoke(() => UpdateHubPlayer?.Invoke(packet.Player));
            logger.Trace("Update player - {0}", packet.Player);
        }
        private void OnUpdateHubPlayerList(StandardServerPlayerlist packet)
        {
            foreach (PlayerInfo infos in packet.Userlist)
            {
                if (infos.Username.ToUpper() == FormExecution.Username.ToUpper())
                    FormExecution.PlayerInfos = infos;
                PlayerManager.UpdatePlayer(infos);
                Application.Current.Dispatcher.Invoke(() => AddHubPlayer?.Invoke(infos, false));
            }
            logger.Trace("UpdatePlayerList - {0}", packet.Userlist);
        }

        private void OnCommandError(StandardServerCommandError packet)
        {
            CommandErrorType type = packet.Type;

            Color c = FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor");
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
                case CommandErrorType.PlayerNotExisted:
                    msg = "••• L'utilisateur ciblé n'existe pas.";
                    break;
                case CommandErrorType.NotEnoughMoney:
                    msg = "••• Tu n'as pas assez de points !";
                    break;
                case CommandErrorType.OpponentNotEnoughMoney:
                    msg = "••• Malheureusement ton adversaire n'a pas assez de points !";
                    break;
                case CommandErrorType.AvatarNotOwned:
                    msg = "••• Tu ne possèdes pas cet avatar !";
                    break;
                case CommandErrorType.BorderNotOwned:
                    msg = "••• Tu ne possèdes pas cette bordure !";
                    break;
                case CommandErrorType.SleeveNotOwned:
                    msg = "••• Tu ne possèdes pas cette sleeve !";
                    break;
                case CommandErrorType.PartnerNotOwned:
                    msg = "••• Tu ne possèdes pas ce partenaire !";
                    break;
                case CommandErrorType.TitleNotOwned:
                    msg = "••• Tu ne possèdes pas ce titre !";
                    break;
                case CommandErrorType.CardNotOwned:
                    msg = "••• Tu ne possèdes pas cette carte !";
                    break;
                case CommandErrorType.PriceUpTo0:
                    msg = "••• Le prix doit être strictement positif.";
                    break;
                case CommandErrorType.QuantityUpTo0:
                    msg = "••• La quantité doit être strictement positif.";
                    break;
                case CommandErrorType.AlreadyInDuel:
                    msg = "Tu es déja en duel.";
                    break;
                case CommandErrorType.PlayerNotInDuel:
                    msg = "••• Ce joueur n'est actuellement pas en duel.";
                    break;
                case CommandErrorType.InvalidRoomPass:
                    msg = "Le mot de passe que tu as entré n'est pas le bon !";
                    break;
                case CommandErrorType.OpponentDisconnected:
                    msg = "Nous avions trouvé un adversaire mais il s'est déconnecté !";
                    break;
                case CommandErrorType.URLTooLong:
                    msg = "L'URL que tu as entré est trop longue (limite 500 caractères) !";
                    break;
                case CommandErrorType.NotEnoughPP:
                    msg = "Tu ne possèdes pas assez de PPs !";
                    break;
                case CommandErrorType.AlreadyBanned:
                    msg = "••• Le joueur est déjà banni !";
                    break;
                case CommandErrorType.TimeTooLong:
                    msg = "••• Tu ne peux pas proposer un temps aussi long !";
                    break;
                case CommandErrorType.LevelTooLow:
                    msg = "••• Tu ne peux pas proposer un duel des ombres à un joueur qui n'est pas niveau 5 ! (Ou alors tu n'es pas niveau 5)";
                    break;
                case CommandErrorType.PlayerAlreadyInTeam:
                    msg = "Tu es déjà dans une autre équipe !";
                    break;
                case CommandErrorType.TargetAlreadyInATeam:
                    msg = "Ce joueur est déjà dans une autre équipe !";
                    break;
                case CommandErrorType.TargetNotInYourTeam:
                    msg = "Ce joueur n'est pas dans ton équipe !";
                    break;
                case CommandErrorType.TeamNameAlreadyExist:
                    msg = "Une équipe du même nom existe déjà !";
                    break;
                case CommandErrorType.TeamTagAlreadyExist:
                    msg = "Une équipe a déjà pris ce tag !";
                    break;
                case CommandErrorType.YouAreAlreadyInATeam:
                    msg = "Tu es déjà dans une équipe !";
                    break;
                case CommandErrorType.YouAreNotTeamCoLeader:
                    msg = "Tu n'es pas co-leader de ton équipe !";
                    break;
                case CommandErrorType.YouAreNotTeamLeader:
                    msg = "Tu n'es pas leader de ton équipe !";
                    break;
                case CommandErrorType.YouHaveNoTeam:
                    msg = "Tu n'as pas d'équipe !";
                    break;
                case CommandErrorType.TeamFull:
                    msg = "Ton équipe est pleine (8 joueurs) !";
                    break;
                case CommandErrorType.CantChangeTeamNow:
                    msg = "Ce joueur ne peut intégrer une équipe actuellement ! Il doit attendre une semaine après avoir quitté une team pour en rejoindre une autre.";
                    break;
                default:
                    msg = "••• Erreur inconnue, impossible à traiter.";
                    break;
            }
            if (packet.MessageBox)
                OpenPopBox(msg, "Erreur");
            else
                Application.Current.Dispatcher.Invoke(() => SpecialChatMessageRecieved?.Invoke(c, msg, italic, bold));
            logger.Trace("COMMAND ERROR MESSAGE MESSAGE - Type : {0} |  Message : {1}", packet.Type, msg);
        }

        public void OnKick(StandardServerKick packet)
        {
            Application.Current.Dispatcher.Invoke(() => PopMessageBoxShowDialog?.Invoke("Vous avez été kické par : " + packet.Kicker + " pour la raison : " + packet.Reason, "Ejection du serveur"));
            logger.Trace("KICKED - By : {0} | Reason : {1}", packet.Kicker, packet.Reason);
        }
        public void OnDisabled(StandardServerDisabledAccount packet)
        {
            Application.Current.Dispatcher.Invoke(() => PopMessageBoxShowDialog?.Invoke("Votre compte a été désactivé par : " + packet.Player + " pour la raison : " + packet.Reason, "Compte désactivé"));
            logger.Trace("DISABLED - By : {0} | Reason : {1}", packet.Player, packet.Reason);
            Application.Current.Dispatcher.Invoke(() => Shutdown?.Invoke());
        }
        public void OnBan(StandardServerBan packet)
        {
            Application.Current.Dispatcher.Invoke(() => PopMessageBoxShowDialog?.Invoke("Vous avez été banni par : " + packet.Banner + " pour la raison : " + packet.Reason + " pour une durée de " + packet.Time + "h.", "Banni du serveur"));
            logger.Trace("BANNED - By : {0} | Time : {1} | Reason : {2}", packet.Banner, packet.Time, packet.Reason);
            Application.Current.Dispatcher.Invoke(() => Shutdown?.Invoke());
        }
        public void OnMute(StandardServerMute packet)
        {
            Application.Current.Dispatcher.Invoke(() => PopMessageBox?.Invoke("Vous avez été rendu muet par : " + packet.Muter + " pour la raison : " + packet.Reason + " pour une durée de " + packet.Time + "h.", "Mute"));
            logger.Trace("MUTED - By : {0} | Time : {1} | Reason : {2}", packet.Muter, packet.Time, packet.Reason);
        }
        public void OnMuted(StandardServerMuted packet)
        {
            Color c = FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor");
            string msg = "Tu es muet ! Jusqu'au " + packet.End + ". Après ce moment, déconnecte toi et reconnecte toi pouvoir de nouveau parler.";
            bool italic = false;
            bool bold = false;
            Application.Current.Dispatcher.Invoke(() => SpecialChatMessageRecieved?.Invoke(c, msg, italic, bold));

            logger.Trace("MUTED - End : {0}", packet.End);
        }
        public void OnUnmute(StandardServerUnmute packet)
        {
            string msg = "Tu as été démuté par " + packet.Player.Username + " !";
            OpenPopBox(msg, "Modération");

            logger.Trace("UNMUTE - By : {0}", packet.Player.Username);
        }
        public void OnMuteChat(StandardServerMutechat packet)
        {
            Application.Current.Dispatcher.Invoke(() => Mutechat?.Invoke(packet.Sender, packet.Reason, packet.IsMuted));
        }

        public void OnPrivateMessage(StandardServerPrivateMessage packet)
        {
            if (BlacklistManager.CheckBlacklist(packet.Player))
                return;

            //packet.Message = ParseUsername(packet.Player.Username, packet.Player.Rank, packet.Player.VIP) + ":" + packet.Message;
            Application.Current.Dispatcher.Invoke(() => PrivateMessageReceived?.Invoke(packet.Player, packet.Message));
            logger.Trace("PRIVATE MESSAGE RECEIVED - From : {0} | Message : {1}", packet.Player, packet.Message);
        }

        public void OnProfilUpdate(StandardServerProfilInfo packet)
        {
            Application.Current.Dispatcher.Invoke(() => ProfilUpdate?.Invoke(packet));
            logger.Trace("PROFIL INFO - Target : {0} |  Infos : {1}", packet.Username, packet);
        }

        public void OnUpdateCollection(StandardServerUpdateCollection packet)
        {
            if (packet.Collection != null)
                PlayerManager.UpdateCollection(packet.Collection);
            else
                return;

            string arg = "-j";

            switch (packet.Reason)
            {
                case AskCollectionReason.Deck_Edit:
                    arg = "-d";
                    break;
                case AskCollectionReason.Duel:
                    arg = "-j";
                    break;
            }
            LaunchYGOProWithoutRoom?.Invoke(arg);
            logger.Trace("UPDATE COLLECTION -  Collection : {0} | Reason : {1}", packet.Collection, packet.Reason);
        }

        public void OnAskBooster(StandardServerAskBooster packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateBoosterInfo?.Invoke(packet.CardGot, packet.TotalCard, packet.Price, packet.CardPerPack, packet.BattlePoints));
            logger.Trace("UPDATE BOOSTER INFO - {0}/{1} | Price : {2} | BP : {3}", packet.CardGot, packet.TotalCard, packet.Price, packet.BattlePoints);
        }
        public void OnPurchaseItem(StandardServerPurchaseItem packet)
        {
            Application.Current.Dispatcher.Invoke(() => PurchaseItem?.Invoke(packet.Cards));
            Application.Current.Dispatcher.Invoke(() => UpdateBattlePoints?.Invoke(packet.Points));
            logger.Trace("PURCHASE ITEMS - {0}", packet.Cards);
        }
        public void OnAskBoosterCollection(StandardServerBoosterCollection packet)
        {
            Application.Current.Dispatcher.Invoke(() => LoadBoosterCollection?.Invoke(packet.PurchaseTag, packet.Id, packet.Quantity, packet.Rarity));
            logger.Trace("ASK BOOSTER COLLECTION - {0}", packet.PurchaseTag);
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
            Application.Current.Dispatcher.Invoke(() => UpdatePanelUser?.Invoke(packet.Profil, packet.Accounts, packet.IP, packet.Observation, packet.Points));
            logger.Trace("UPDATE PANEL PLAYER - Players : {0} | IP : {1} | Observation : {2} | Points : {3}", packet.Accounts, packet.IP, packet.Observation, packet.Points);
        }

        public void OnBanlist(StandardServerBanlist packet)
        {
            Application.Current.Dispatcher.Invoke(() => Banlist?.Invoke(packet.Players));
            logger.Trace("BANLIST - {0}", packet.Players);
        }
        public void OnMutelist(StandardServerMutelist packet)
        {
            Application.Current.Dispatcher.Invoke(() => Mutelist?.Invoke(packet.Players));
            logger.Trace("MUTELIST - {0}", packet.Players);
        }

        public void OnGivePoints(StandardServerGetPoints packet)
        {
            string type = "Battle";
            if (packet.PrestigePoints)
                type = "Prestige";
            OpenPopBox("Vous avez reçu " + packet.Points + " " + type + " points de la part de " + packet.Player.Username, "Réception de points");
            logger.Trace("GET POINTS - From : {0} | Amount : {1} | Prestige : {2}", packet.Player.Username, packet.Points, packet.PrestigePoints);
        }
        public void OnGiveCard(StandardServerGetCard packet)
        {
            OpenPopBox("Vous avez reçu la carte : " + CardManager.GetCard(packet.Id).Name + " de la part de " + packet.Player.Username, "Réception de carte");
            logger.Trace("GET CARD - From : {0} | Id : {1}", packet.Player.Username, packet.Id);
        }
        public void OnGiveAvatar(StandardServerGetAvatar packet)
        {
            Application.Current.Dispatcher.Invoke(() => CustomizationAchievement?.Invoke(CustomizationType.Avatar, "Vous avez reçu l'avatar : " + packet.Id + " de la part de " + packet.Player.Username, packet.Id));
            logger.Trace("GET AVATAR - From : {0} | Id : {1}", packet.Player.Username, packet.Id);
        }
        public void OnGiveBorder(StandardServerGetBorder packet)
        {
            Application.Current.Dispatcher.Invoke(() => CustomizationAchievement?.Invoke(CustomizationType.Border, "Vous avez reçu la bordure : " + packet.Id + " de la part de " + packet.Player.Username, packet.Id));
            logger.Trace("GET BORDER - From : {0} | Id : {1}", packet.Player.Username, packet.Id);
        }
        public void OnGiveSleeve(StandardServerGetSleeve packet)
        {
            Application.Current.Dispatcher.Invoke(() => CustomizationAchievement?.Invoke(CustomizationType.Sleeve, "Vous avez reçu la sleeve : " + packet.Id + " de la part de " + packet.Player.Username, packet.Id));
            logger.Trace("GET SLEEVE - From : {0} | Id : {1}", packet.Player.Username, packet.Id);
        }
        public void OnGivePartner(StandardServerGetPartner packet)
        {
            Application.Current.Dispatcher.Invoke(() => CustomizationAchievement?.Invoke(CustomizationType.Partner, "Vous avez reçu le partenaire : " + packet.Id + " de la part de " + packet.Player.Username, packet.Id));
        }
        public void OnCardDonation(StandardServerCardDonation packet)
        {
            List<string> cards = new List<string>();
            foreach (var card in packet.Cards)
                cards.Add(card.Value.ToString());

            OpenPopBox("Vous avez reçu les cartes : " + string.Join(",", cards) + " de la part de " + packet.Player.Username, "Réception d'une carte");
            logger.Trace("GET CARD - From : {0} | Ids : {1}", packet.Player.Username, string.Join(",", cards));
        }

        public void OnTradeRequest(StandardServerTradeRequest packet)
        {
            if (BlacklistManager.CheckBlacklist(packet.Player) || FormExecution.ClientConfig.IgnoreTradeRequest)
            {
                Send(PacketType.TradeRequestAnswer, new StandardClientTradeRequestAnswer { Player = packet.Player, Result = false });
                return;
            }

            Application.Current.Dispatcher.Invoke(() => ChoicePopBox?.Invoke(packet.Player, new RoomConfig(), ChoiceBoxType.Trade, ""));
            logger.Trace("Trade REQUEST - From {0}", packet.Player.Username);
        }
        public void OnTradeRequestAnswer(StandardServerTradeRequestAnswer packet)
        {
            Color c = FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor");
            logger.Trace("Trade REQUEST ANSWER - From {0} | Result : {1}", packet.Player.Username, packet.Result);
            try
            {
                if (packet.Result)
                {
                    Application.Current.Dispatcher.Invoke(() => LaunchTrade?.Invoke());
                    Application.Current.Dispatcher.Invoke(() => InitTrade?.Invoke(packet.Id, new PlayerInfo[] { FormExecution.PlayerInfos, packet.Player }, packet.Collections));
                }
                else
                    Application.Current.Dispatcher.Invoke(() => SpecialChatMessageRecieved?.Invoke(c, "••• " + packet.Player.Username + " a refusé votre échange.", false, false));
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }
        public void OnTradeMessage(StandardServerTradeMessage packet)
        {
            logger.Trace("TRADE MESSAGE - From {0} | Message : {1}", packet.Player.Username, packet.Message);
            Application.Current.Dispatcher.Invoke(() => GetMessage?.Invoke(packet.Player, packet.Message));
        }
        public void OnTradeProposition(StandardServerTradeProposition packet)
        {
            logger.Trace("TRADE PROPOSITION - Cards {0}", packet.Cards.ToArray());
            Application.Current.Dispatcher.Invoke(() => UpdateCardsToOffer?.Invoke(packet.Cards));

        }
        public void OnTradeExit(StandardServerTradeExit packet)
        {
            logger.Trace("TRADE Exit }");
            Application.Current.Dispatcher.Invoke(() => TradeExit?.Invoke());
        }
        public void OnTradeEnd(StandardServerTradeExit packet)
        {
            logger.Trace("TRADE END }");
            Application.Current.Dispatcher.Invoke(() => TradeEnd?.Invoke());
        }

        public void OnLoadAvatars(StandardServerLoadAvatars packet)
        {
            logger.Trace("LOAD AVATARS - Ids : {0}", packet.Avatars);
            Application.Current.Dispatcher.Invoke(() => LoadAvatars?.Invoke(packet.Avatars));
        }
        public void OnLoadBorders(StandardServerLoadBorders packet)
        {
            logger.Trace("LOAD BORDERS - Ids : {0}", packet.Borders);
            Application.Current.Dispatcher.Invoke(() => LoadBorders?.Invoke(packet.Borders));
        }
        public void OnLoadSleeves(StandardServerLoadSleeves packet)
        {
            logger.Trace("LOAD SLEEVES - Ids : {0}", packet.Sleeves);
            Application.Current.Dispatcher.Invoke(() => LoadSleeves?.Invoke(packet.Sleeves));
        }
        public void OnLoadPartners(StandardServerLoadPartners packet)
        {
            logger.Trace("LOAD PARTNERS - Ids : {0}", packet.Partners);
            Application.Current.Dispatcher.Invoke(() => LoadPartners?.Invoke(packet.Partners));
        }

        public void OnLoadBrocante(StandardServerLoadBrocante packet)
        {
            logger.Trace("LOAD BROCANTE");
            Application.Current.Dispatcher.Invoke(() => LoadBrocante?.Invoke(packet.Cards));
        }
        public void OnLoadSelectCard(StandardServerLoadSelectCard packet)
        {
            logger.Trace("LOAD SELECT CARD");
            switch (packet.Reason)
            {
                case AskCollectionReason.Brocante:
                    Application.Current.Dispatcher.Invoke(() => LoadSelectCard?.Invoke(packet.Collection, packet.CardSold));
                    break;
                case AskCollectionReason.GiveCard:
                    Application.Current.Dispatcher.Invoke(() => LoadGiveCards?.Invoke(packet.Collection));
                    break;
            }
        }
        public void OnCloseBrocante(StandardServerCloseBrocante packet)
        {
            Application.Current.Dispatcher.Invoke(() => CloseBrocante?.Invoke());

            logger.Trace("CLOSE BROCANTE");
        }

        public void OnSearchCard(StandardServerSearchCard packet)
        {
            string boosters = "La carte est disponible dans les boosters : ";
            if (packet.Boosters.Count == 0)
                boosters += "Aucun.";
            else
            {
                foreach (string booster in packet.Boosters)
                {
                    BoosterInfo BoosterChoosen = BoosterManager.InitializeBooster("(" + booster + ")");
                    boosters += Environment.NewLine + BoosterChoosen.Name + " (" + BoosterChoosen.PurchaseTag + ")";
                }
            }
            Application.Current.Dispatcher.Invoke(() => PopMessageBox?.Invoke(boosters, "Recherche de carte"));

            logger.Trace("SEARCH CARD - Answer : {0}", packet.Boosters.ToArray().ToString());
        }

        public void OnDuelStart(StandardServerDuelStart packet)
        {
            string arg = "-j " + FormExecution.GetIp() + " " + packet.Room.Id;
            YgoproConfig.UpdateNickname(FormExecution.Username);
            YgoproConfig.UpdateForced(packet.Room.IsRanked());

            Application.Current.Dispatcher.Invoke(() => LaunchYGOPro?.Invoke(packet.Room, arg));

            logger.Trace("DUEL START - Id : {0} | Type : {1} | Players : {2}", packet.Room.Id, packet.Room.Config.Type, packet.Room.Players);
        }
        public void OnDuelSpectate(StandardServerDuelSpectate packet)
        {
            string arg = "-j " + FormExecution.GetIp() + " " + packet.Room.Id;
            YgoproConfig.UpdateNickname(FormExecution.Username);

            LaunchYGOPro?.Invoke(packet.Room, arg);

            logger.Trace("DUEL SPECTATE - Id : {0} | Type : {1} | Players : {2}", packet.Room.Id, packet.Room.Config.Type, packet.Room.Players);
        }
        public void OnDuelRequest(StandardServerDuelRequest packet)
        {
            if (BlacklistManager.CheckBlacklist(packet.Player) || FormExecution.ClientConfig.IgnoreDuelRequest)
            {
                Send(PacketType.DuelRequestAnswer, new StandardClientDuelRequestAnswer { Player = packet.Player, Config = packet.Config, Roompass = packet.RoomPass, Result = false });
                return;
            }
            if (packet.Config.IsCustom() && FormExecution.ClientConfig.IgnoreCustomDuelRequest)
            {
                Send(PacketType.DuelRequestAnswer, new StandardClientDuelRequestAnswer { Player = packet.Player, Config = packet.Config, Roompass = packet.RoomPass, Result = false });
                return;
            }

            logger.Trace("DUEL REQUEST - From {0} | Type : {1}", packet.Player.Username, packet.Config.Type);

            Application.Current.Dispatcher.Invoke(() => ChoicePopBox?.Invoke(packet.Player, packet.Config, ChoiceBoxType.Duel, packet.RoomPass));
        }
        public void OnDuelRequestAnswer(StandardServerDuelRequestResult packet)
        {
            Color c = FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor");
            logger.Trace("DUEL REQUEST ANSWER - From {0} | Result : {1}", packet.Player.Username, packet.Result);
            if (!packet.Result)
                Application.Current.Dispatcher.Invoke(() => SpecialChatMessageRecieved?.Invoke(c, "••• " + packet.Player.Username + " a refusé votre duel.", false, false));
        }
        public void OnShadowDuelRequest(StandardServerShadowDuelRequest packet)
        {
            if (FormExecution.ClientConfig.FirstTimeShadowDuel)
            {
                FormExecution.Client_PopMessageBoxShowDialog(StartDisclaimer.ShadowDuelText, "Premier duel des ombres !");
                FormExecution.ClientConfig.FirstTimeShadowDuel = false;
                FormExecution.ClientConfig.Save();
            }

            if (BlacklistManager.CheckBlacklist(packet.Player) || FormExecution.ClientConfig.IgnoreDuelRequest || FormExecution.ClientConfig.IgnoreShadowDuelRequest)
            {
                Send(PacketType.ShadowDuelRequestAnswer, new StandardClientShadowDuelAnswer { Player = packet.Player, Config = packet.Config, Result = false });
                return;
            }
            if (packet.Config.IsCustom() && FormExecution.ClientConfig.IgnoreCustomDuelRequest)
            {
                Send(PacketType.ShadowDuelRequestAnswer, new StandardClientShadowDuelAnswer { Player = packet.Player, Config = packet.Config, Result = false });
                return;
            }

            logger.Trace("SHADOW DUEL REQUEST - From {0} | Type : {1}", packet.Player.Username, packet.Config.Type);

            Bet b = null;
            switch (packet.BType)
            {
                case BetType.BPs:
                    b = JsonConvert.DeserializeObject<BPsBet>(packet.BetSerealized);
                    break;
                case BetType.Mute:
                case BetType.Ban:
                    b = JsonConvert.DeserializeObject<SanctionBet>(packet.BetSerealized);
                    break;
            }
            if (b == null)
                return;

            Application.Current.Dispatcher.Invoke(() => ShadowDuelRequest?.Invoke(packet.Player, packet.Config, b));
        }
        public void OnShadowDuelRequestAnswer(StandardServerShadowDuelRequestResult packet)
        {
            Color c = FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor");
            logger.Trace("DUEL REQUEST ANSWER - From {0} | Result : {1}", packet.Player.Username, packet.Result);
            if (!packet.Result)
                Application.Current.Dispatcher.Invoke(() => SpecialChatMessageRecieved?.Invoke(c, "••• " + packet.Player.Username + " a refusé votre duel des ombres.", false, false));
        }
        public void OnUpdateRoom(StandardServerUpdateRoom packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateRoom?.Invoke(packet.Room));
            if (packet.Room.State != RoomState.Waiting)
                Application.Current.Dispatcher.Invoke(() => UpdateHubPlayers?.Invoke(packet.Room.Players, packet.Room.State == RoomState.Dueling ? PlayerState.Duel : PlayerState.Lobby));
            logger.Trace("UPDATE ROOM - Id : {0} | Type : {1} | Players : {2}", packet.Room.Id, packet.Room.Config.Type, packet.Room.Players);
        }
        public void OnRoomNeedPassword(StandardServerNeedRoomPassword packet)
        {
            logger.Trace("ROOM NEED PASSWORD - Room {0} | Type : {1}", packet.RoomId, packet.RoomType);
            Application.Current.Dispatcher.Invoke(() => RoomNeedPassword?.Invoke(packet.RoomId, packet.RoomType));
        }

        public void OnLoadPlayerCustomizationTextures(StandardServerLoadPlayerCustomizationTextures packet)
        {
            Application.Current.Dispatcher.Invoke(() => LoadPlayerCustomizations?.Invoke(packet.Avatar, packet.Border, packet.Sleeve, packet.Partner, packet.Team, packet.TeamEmblem, packet.Pos));
            logger.Trace("LOAD PLAYER CUSTOMIZATION TEXTURES");
        }

        public void OnGetBonus(StandardServerGetBonus packet)
        {
            Application.Current.Dispatcher.Invoke(() => LaunchBonusBox?.Invoke(packet.Type, packet.MonthlyConnectionNumber, packet.Gift, packet.Cards));
            logger.Trace("GET BONUS - Bonus Type : {0} | Monthly connection : {1} | Gift : {2}", packet.Type, packet.MonthlyConnectionNumber, packet.Gift);
        }
        public void OnGetMonthlyBonus(StandardServerGetMonthlyBonus packet)
        {
            Application.Current.Dispatcher.Invoke(() => GetMonthlyBonus?.Invoke(packet.Bonus, packet.MonthlyConnectionNumber, packet.Cards));
            logger.Trace("GET BONUS - Bonus Type : {0} | Monthly connection : {1}", packet.Bonus, packet.MonthlyConnectionNumber);
        }

        public void OnDuelResult(StandardServerDuelResult packet)
        {
            if (FormExecution.ClientConfig.PMEndDuel)
                Application.Current.Dispatcher.Invoke(() => LaunchDuelResultBox?.Invoke(packet.PointsGain, packet.ExpGain, packet.Win, packet.Opponent, packet.Config, packet.RoomID));
            else
            {
                Color c = FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor");
                string text = "••• ";
                if (packet.Win)
                    text = "Félicitations pour ta victoire !";
                else
                    text = "Dommage tu viens de perdre... Tu feras mieux la prochaine fois !";
                text += Environment.NewLine + Environment.NewLine;

                text += "Tu as remporté " + packet.PointsGain.ToString() + " BPs et " + packet.ExpGain.ToString() + " points d'expériences.";
                Application.Current.Dispatcher.Invoke(() => SpecialChatMessageRecieved?.Invoke(c, text, false, false));

                DuelResultAdmin.SendDuelResultAnswer(false, packet.Config, packet.Opponent, packet.RoomID);
            }

            logger.Trace("DUEL RESULT - BPs Gain : {0} | EXPs Gain : {1} | Win : {2}", packet.PointsGain, packet.ExpGain, packet.Win);
        }
        public void OnDuelResultAnswer(StandardServerDuelResultAnswer packet)
        {
            Application.Current.Dispatcher.Invoke(() => DuelResultAnswer?.Invoke(packet.Result));
        }

        public void OnGetTitle(StandardServerGetTitle packet)
        {
            OpenPopBox("Vous avez reçu le titre : " + packet.Title + " de la part de " + packet.Player.Username, "Réception d'un titre");
            logger.Trace("GET TITLE - From : {0} | Id : {1}", packet.Player.Username, packet.Title);
        }
        public void OnLoadTitles(StandardServerLoadTitles packet)
        {
            logger.Trace("LOAD TITLES - Ids : {0}", packet.Titles.Keys);
            Application.Current.Dispatcher.Invoke(() => LoadTitles?.Invoke(packet.Titles));
        }

        public void OnMaintenance(StandardServerMaintenance packet)
        {
            Application.Current.Dispatcher.Invoke(() => PopMessageBoxShowDialog?.Invoke("Une maintenance va démarrer, vous allez être kické du serveur." + Environment.NewLine + "Raison: " + packet.Reason + Environment.NewLine + "Temps estimé: " + packet.TimeEstimation.ToString() + "h.", "Maintenance"));
            logger.Trace("MAINTENANCE - Reason : {0} | Time Estimation : {1}", packet.Reason, packet.TimeEstimation);
            Application.Current.Dispatcher.Invoke(() => Shutdown?.Invoke());
        }

        public void OnOfflineMessages(StandardServerOfflineMessages packet)
        {
            logger.Trace("LOAD OFFLINE MESSAGES - Messages : {0}", packet.Messages);
            Application.Current.Dispatcher.Invoke(() => LoadOfflineMessages?.Invoke(packet.Messages));
        }

        public void OnResetStat(StandardServerResetStat packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            OpenPopBox("Tes stats viennent d'être réinitialisées !", "Réinitialisation des stats");
            logger.Trace("RESET STAT RECIEVED");
        }
        public void OnChangeUsername(StandardServerChangeUsername packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            FormExecution.Username = packet.Username;
            OpenPopBox("Ton pseudo a été changé ! Nouveau pseudo : " + packet.Username, "Changement de pseudo");
            logger.Trace("CHANGE USERNAME - New username : {0}", packet.Username);
        }
        public void OnChangeChatColor(StandardServerUsernameColor packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            OpenPopBox("Tu as désormais une nouvelle couleur dans le chat, va l'essayer !", "Changement de couleur de pseudo");
            logger.Trace("CHANGE USERNAME COLOR");
        }
        public void OnBuyMonthPack(StandardServerBuyMonthPack packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            OpenPopBox("Félicitations ! Tu viens d'obtenir le pack du mois, contenant un avatar, une bordure, une sleeve et t'offrant un jour de multiplication par deux de tes gains de BPs en duel ainsi qu'un mois VIP !", "Pack du mois !");
            FormExecution.AddNotes("Fin de la période VIP : " + DateTime.Now.AddMonths(1) + ". Date d'achat : " + DateTime.Now);
            logger.Trace("BUY MONTH PACK");
        }
        public void OnAskMonthPack(StandardServerAskMonthPack packet)
        {
            Application.Current.Dispatcher.Invoke(() => LoadMonthPack?.Invoke(packet.Avatar, packet.Border, packet.Sleeve, packet.Partner));
            logger.Trace("ASK MONTH PACK");
        }
        public void OnBuyOwnCustomization(StandardServerBuyOwnCustomization packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            OpenPopBox("Félicitations ! Tu viens d'obtenir pour une durée d'un mois ta customisation personnalisée ! Va vite l'essayer dans ton profil !" + Environment.NewLine + "Tu pourras l'utiliser jusqu'au " + DateTime.Now.AddMonths(1) + ". Cette date est stockée dans ton bloc-note !", "Customisation personnalisée !");
            FormExecution.AddNotes("Fin de ta customisation personnalisée : " + DateTime.Now.AddMonths(1) + ". Date d'achat : " + DateTime.Now);
            logger.Trace("BUY OWN CUSTOMIZATION");
        }
        public void OnBuyPrestigeCustomization(StandardServerBuyPrestigeCustomization packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            OpenPopBox("Félicitations ! Tu viens d'obtenir pour une customisation de prestige ! Va vite l'essayer dans ton profil !", "Customisation de prestige !");
            logger.Trace("BUY PRESTIGE CUSTOMIZATION");
        }
        public void OnBuyVIP(StandardServerBuyVIP packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            OpenPopBox("Félicitations ! Tu viens de devenir VIP pour une période de 3 mois ! Cela te permet de doubler tes BPs en animation et de possèder un avatar, une bordure et une sleeve réservés aux VIPs !" + Environment.NewLine + "Tu le seras jusqu'au " + DateTime.Now.AddMonths(3) + ". Cette date est indiquée dans ton bloc note sur le jeu !", "Nouveau VIP !");
            FormExecution.AddNotes("Fin de la période VIP : " + DateTime.Now.AddMonths(3) + ". Date d'achat : " + DateTime.Now);
            logger.Trace("BUY VIP");
        }
        public void OnBuyDoubleBP(StandardServerDoubleBP packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            OpenPopBox("Félicitations ! Pendant 3 jours, tes gains de BPs en duel vont être doublés !" + Environment.NewLine + "Tu pourras en profiter jusqu'au " + DateTime.Now.AddDays(3), "Double de BPs !");
            FormExecution.AddNotes("Fin de la période double BP : " + DateTime.Now.AddDays(3) + ". Date d'achat : " + DateTime.Now);
            logger.Trace("BUY DOUBLE BP");
        }
        public void OnBuyInfiniteGreet(StandardServerGreetInfinite packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            OpenPopBox("Félicitations ! Tu vas désormais pouvoir changer de greet à l'infini !", "Greet infini !");
            logger.Trace("BUY INIFINITE GREET");
        }
        public void OnBuySpecialPack(StandardServerBuySpecialPack packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            OpenPopBox("Félicitations ! Tu viens d'obtenir un pack spécial disponible durant une durée limitée seulement !", "Pack special !");
            logger.Trace("BUY SPECIAL PACK");
        }
        public void OnBuyCreateTeam(StandardServerCreateTeam packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            OpenPopBox("Félicitations ! Tu viens de créer ta team ! Fonce affrontez les autres !", "New TEAM !");
            logger.Trace("BUY CREATE TEAM");
        }

        public void OnOpenPrestigeShop(StandardServerOpenPrestigeShop packet)
        {
            Application.Current.Dispatcher.Invoke(() => UpdatePP?.Invoke(packet.PP));
            Application.Current.Dispatcher.Invoke(() => UpdateProgress?.Invoke(packet.Progress));
            if (packet.SpecialPack != null)
                Application.Current.Dispatcher.Invoke(() => UpdateSpecialPack?.Invoke(packet.SpecialPack));
            logger.Trace("Open Prestige Shop");
        }

        public void OnCustomizationAchievement(StandardServerGetCustomizationAchievement packet)
        {
            string txt = "Vous venez de débloquer une personnalisation via une quête !" + Environment.NewLine;
            switch (packet.CustomType)
            {
                case CustomizationType.Avatar:
                    txt += "L'avatar n°";
                    break;
                case CustomizationType.Border:
                    txt += "La bordure n°";
                    break;
                case CustomizationType.Sleeve:
                    txt += "La sleeve n°";
                    break;
                case CustomizationType.Partner:
                    txt += "Le partenaire n°";
                    break;
                case CustomizationType.Title:
                    txt += "Le titre n°";
                    break;
            }
            txt += packet.Id;
            switch (packet.AchievementType)
            {
                case CustomizationAchievementType.LevelUP:
                    txt += " grâce à ta montée au niveau " + packet.Amount + " ! ";
                    break;
                case CustomizationAchievementType.BuyAllBooster:
                    txt += " car tu possèdes la totalité des cartes du booster : " + packet.BoosterTag + " ! ";
                    break;
                case CustomizationAchievementType.ELODown:
                    txt += " car ton ELO a chuté en dessous de " + packet.Amount + "...";
                    break;
                case CustomizationAchievementType.ELOUp:
                    txt += " car ton ELO est supérieur à " + packet.Amount + " ! ";
                    break;
                case CustomizationAchievementType.MatchWin:
                    txt += " car tu as remporté plus de " + packet.Amount + " matchs ! ";
                    break;
                case CustomizationAchievementType.RankedWin:
                    txt += " car tu as remporté plus de " + packet.Amount + " matchs classés ! ";
                    break;
                case CustomizationAchievementType.SingleWin:
                    txt += " car tu as remporté plus de " + packet.Amount + " singles ! ";
                    break;
                case CustomizationAchievementType.TagWin:
                    txt += " car tu as remporté plus de " + packet.Amount + " tags ! ";
                    break;
                case CustomizationAchievementType.RankedLose:
                    txt += " car tu as perdu plus de " + packet.Amount + " matchs classés ! ";
                    break;
                case CustomizationAchievementType.MatchLose:
                    txt += " car tu as perdu plus de " + packet.Amount + " matchs ! ";
                    break;
                case CustomizationAchievementType.SingleLose:
                    txt += " car tu as perdu plus de " + packet.Amount + " singles ! ";
                    break;
                case CustomizationAchievementType.TagLose:
                    txt += " car tu as perdu plus de " + packet.Amount + " tags ! ";
                    break;
                case CustomizationAchievementType.UnlockCards:
                    txt += " car tu possèdes plus de " + packet.Amount + " cartes !";
                    break;
                case CustomizationAchievementType.SURPRISE:
                    txt += " car tu as accompli une quête secrète !";
                    break;
                case CustomizationAchievementType.Event:
                    txt += " car aujourd'hui est un jour particulier !";
                    break;
                case CustomizationAchievementType.AvatarCount:
                    txt += " car tu as débloqué " + packet.Amount + " avatars !";
                    break;
                case CustomizationAchievementType.TitleCount:
                    txt += " car tu as débloqué " + packet.Amount + " titres !";
                    break;
                case CustomizationAchievementType.RageQuit:
                    txt += " car tu as effectué " + packet.Amount + " rage quit !";
                    break;
                case CustomizationAchievementType.GiveUp:
                    txt += " car tu as abandonné " + packet.Amount + " fois !";
                    break;
                case CustomizationAchievementType.PPAmount:
                    txt += " car tu as désormais plus de " + packet.Amount + " PPs !";
                    break;
                case CustomizationAchievementType.BPAmount:
                    txt += " car tu as désormais plus de " + packet.Amount + " BPs !";
                    break;
                case CustomizationAchievementType.GlobalDuelLose:
                    txt += " car tu as perdu au total " + packet.Amount + " duels !";
                    break;
                case CustomizationAchievementType.GlobalDuelMade:
                    txt += " car tu as fait au total " + packet.Amount + " duels !";
                    break;
                case CustomizationAchievementType.GlobalDuelWin:
                    txt += " car tu as gagné au total " + packet.Amount + " duels !";
                    break;
                case CustomizationAchievementType.PlayerSeniority:
                    txt += " car tu as créé ton compte il y a plus de " + packet.Amount + " jours !";
                    break;
                case CustomizationAchievementType.Ranking:
                    txt += " car tu as terminé à la " + packet.Amount + (packet.Amount != 1 ? "ème" : "er") + " place du classement de la saison !";
                    break;
                default:
                    txt += " car ... aucune idée !";
                    break;
            }
            txt += Environment.NewLine + "Va vite voir ce nouvel élément dans ton profil !";
            if (packet.CustomType != CustomizationType.Title)
                Application.Current.Dispatcher.Invoke(() => CustomizationAchievement?.Invoke(packet.CustomType, txt, packet.Id));
            else
                OpenPopBox(txt, "Quête accomplie !");
            logger.Trace("GET CUSTOMIZATION ACHIEVEMENT  - Id : {0} | Type : {1} | Custom Type {2}", packet.Id, packet.AchievementType, packet.CustomType);
        }

        public void OnLoadPrestigeCustomizations(StandardServerLoadPrestigeCustomizations packet)
        {
            switch (packet.CType)
            {
                case CustomizationType.Avatar:
                case CustomizationType.Sleeve:
                case CustomizationType.Border:
                case CustomizationType.Partner:
                case CustomizationType.Title:
                    Application.Current.Dispatcher.Invoke(() => LoadPrestigeCustomizations?.Invoke(packet.Customizations));
                    break;
            }

            logger.Trace("LOAD PRESTIGE CUSTOMIZATIONS - Type : {0}", packet.CType);
        }

        public void OnDataRetrievalInfos(StandardServerDataRetrieval packet)
        {
            Application.Current.Dispatcher.Invoke(() => DataRetrievalInfos?.Invoke(packet.Success, packet.Reason, packet.End));
            logger.Trace("DATA RETRIEVAL - Packet : {0}", packet);
        }

        public void OnNextRankingSeason(StandardServerNextRankingSeason packet)
        {
            logger.Trace("NEW RANKING SEASON - {0}", packet.Season);

            Color c = FormExecution.AppDesignConfig.GetGameColor("InformationMessageColor");
            string msg = "Une nouvelle saison de classement a commencé ! (Classement n°" + packet.Season + ")";
            bool italic = false;
            bool bold = false;
            Application.Current.Dispatcher.Invoke(() => SpecialChatMessageRecieved?.Invoke(c, msg, italic, bold));
        }

        public void OnShareDeck(StandardServerSendDeck packet)
        {
            if (BlacklistManager.CheckBlacklist(packet.Sender))
                return;

            Application.Current.Dispatcher.Invoke(() => RecieveDeck?.Invoke(packet.Sender, packet.Deckfile, packet.Deckname));
            logger.Trace("Recieve deck - From : {0}", packet.Sender);
        }
        public void OnShareReplay(StandardServerShareReplay packet)
        {
            if (BlacklistManager.CheckBlacklist(packet.Sender))
                return;

            Application.Current.Dispatcher.Invoke(() => RecieveReplay?.Invoke(packet.Sender, packet.ReplayFile, packet.ReplayName));
            logger.Trace("Recieve replay - From : {0}", packet.Sender);
        }

        public void OnGetRanking(StandardServerGetRanking packet)
        {
            Application.Current.Dispatcher.Invoke(() => ShowRanking?.Invoke(packet.Rankings, packet.PodiumCustoms, packet.Season));

            logger.Trace("RECIEVE RANKING");

        }
        public void OnGetGamesHistory(StandardServerGamesHistory packet)
        {
            Application.Current.Dispatcher.Invoke(() => GetGamesHistory?.Invoke(packet.Results));

            logger.Trace("RECIEVE GAMES HISTORY");
        }

        public void OnGetDailyQuests(StandardServerSendDailyQuests packet)
        {
            Application.Current.Dispatcher.Invoke(() => GetDailyQuests?.Invoke(packet.DQTypes, packet.Quests, packet.States));
        }
        public void OnDailyQuestReward(StandardServerGetDailyQuestReward packet)
        {
            Application.Current.Dispatcher.Invoke(() => DailyQuestReward?.Invoke(packet.Success, packet.DQType, packet.Reward, packet.AllQuests));
        }
        public void OnChangeDailyQuest(StandardServerChangeDailyQuest packet)
        {
            Application.Current.Dispatcher.Invoke(() => ChangeDailyQuest?.Invoke(packet.Success, packet.DQType, packet.NewQuest));
        }
        public void OnDailyQuestNotification(StandardServerDailyQuestNotification packet)
        {
            Application.Current.Dispatcher.Invoke(() => DailyQuestNotification?.Invoke());
        }

        public void OnDuelServerStop(StandardServerDuelServerStop packet)
        {
            Application.Current.Dispatcher.Invoke(() => PopMessageBox?.Invoke("Le serveur de duel est actuellement arrêté ! Ca ne devrait pas durer longtemps. ( Raison : " + packet.Reason + ")", "Serveur de duel arrêté"));
        }

        public void OnGetAnimations(StandardServerGetAnimations packet)
        {
            Application.Current.Dispatcher.Invoke(() => LoadAnimations?.Invoke(packet.Colors, packet.Animations));
        }
        public void OnAnimationNotification(StandardServerAnimationNotification packet)
        {
            Application.Current.Dispatcher.InvokeAsync(() => AnimationNotification?.Invoke(packet.Update));
        }

        public void OnRecieveRoomIsWaiting(StandardServerWaitingRoom packet)
        {
            if (!FormExecution.ClientConfig.ShowArenaWaitingRoomMessage)
                return;
            Color c = FormExecution.AppDesignConfig.GetGameColor("DuelArenaMessageColor");
            string msg = "⚔️" + packet.WaitingRoom.Players[0].Username + " attend un adversaire dans l'arène pour un " + packet.WaitingRoom.Config.Type + " ! ⚔️";
            Application.Current.Dispatcher.InvokeAsync(() => SpecialChatMessageRecieved?.Invoke(c, msg, false, false));
        }

        public void OnGetRoomsList(StandardServerRoomList packet)
        {
            Application.Current.Dispatcher.InvokeAsync(() => GetRoomsList?.Invoke(packet.Rooms));
        }

        public void OnTeamProfil(StandardServerAskTeam packet)
        {
            Application.Current.Dispatcher.InvokeAsync(() => TeamProfileInfos?.Invoke(packet));
        }
        public void OnTeamsRanking(StandardServerGetTeamsRanking packet)
        {
            Application.Current.Dispatcher.InvokeAsync(() => ShowTeamRanking?.Invoke(packet.Ranking, packet.Season));
        }
        public void OnTeamsGameHistory(StandardServerTeamGamesHistory packet)
        {
            Application.Current.Dispatcher.InvokeAsync(() => TeamGamesHistory?.Invoke(packet.Results));
        }

        public void OnGetOpenWorldCharacters(StoryModeServerGetOpenWorldCharacters packet)
        {
            Application.Current.Dispatcher.InvokeAsync(() => GetOpenWorldCharacters?.Invoke(packet.Characters));
        }
        public void OnGetScenes(StoryModeServerGetScenes packet)
        {
            Application.Current.Dispatcher.InvokeAsync(() => GetScenes?.Invoke(packet.Scenes));
        }

        public void OnPing(StandardServerPing packet)
        {
            this.Send(PacketType.Ping, new StandardClientPong { });
        }

        public string ParseUsernames(string username, PlayerRank rank, bool isVip)
        {
            /*switch (rank)
            {
                case PlayerRank.Owner:
                    return "♛" + username;
                case PlayerRank.Bot:
                    return "☎" + username;
                case PlayerRank.Moderateurs:
                    return "♝" + username;
                case PlayerRank.Animateurs:
                    return "♞" + username;
                case PlayerRank.Developper:
                    return "♣" + username;
                case PlayerRank.Contributor:
                    return "♟" + username;
                default: */
            if (isVip)
                return "✮" + username;
            else
                return username;
        }

        public void SendRoomNeedPassword(int id, string pass, RoomType type)
        {
            Send(PacketType.NeedRoomPassword, new StandardClientRoomNeedPassword
            {
                RoomId = id,
                Roompass = pass,
                RoomType = type
            });
        }
    }

}