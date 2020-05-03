using Newtonsoft.Json;
using System.IO;

namespace hub_client.Configuration
{
    public class ClientConfig
    {
        public bool TestMode = false;

        public bool Greet = true;
        public bool Trade = false;
        public bool Request = false;
        public bool AllowDeckShare = true;
        public bool Connexion_Message = true;
        public bool Autoscroll = true;
        public bool PMPopup = true;
        public bool PMEndDuel = true;
        public bool ShowChatScrollbar = false;
        public bool UserlistScrollbar = false;

        public bool BCA_Card_Design = false;

        public int CardsStuffVersion = 0;

        public void Save()
        {
            File.WriteAllText(FormExecution.ClientConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
