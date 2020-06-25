using Newtonsoft.Json;
using System.IO;

namespace hub_client.Configuration
{
    public class ClientConfig
    {
        public bool TestMode = false;

        public bool FirstTimeRanked = true;
        public bool FirstTimeBrocante = true;
        public bool FirstTimeShadowDuel = true;

        public bool Greet = true;
        public bool IgnoreTradeRequest = false;
        public bool IgnoreDuelRequest = false;
        public bool IgnoreCustomDuelRequest = false;
        public bool IgnoreShadowDuelRequest = false;
        public bool AllowDeckShare = true;
        public bool AllowReplayShare = true;
        public bool Connexion_Message = true;
        public bool Autoscroll = true;
        public bool PMPopup = true;
        public bool PMEndDuel = true;
        public bool ShowChatScrollbar = false;
        public bool UserlistScrollbar = false;
        public bool TradeScrollBar = false;
        public bool ShowArenaWaitingRoomMessage = true;

        public bool AlternativePurchaseWindow = false;

        public bool BCA_Card_Design = false;

        public bool ChatBackgroundIsPic = false;

        public int CardsStuffVersion = 0;

        public bool DoTutoChat = true;
        public bool DoTutoArena = true;
        public bool DoTutoShop = true;
        public bool DoTutoQuests = true;

        public void Save()
        {
            File.WriteAllText(FormExecution.ClientConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
