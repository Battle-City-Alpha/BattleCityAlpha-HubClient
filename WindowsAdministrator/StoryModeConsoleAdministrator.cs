using BCA.Network.Packets.Enums;
using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCA.Network.Packets.Story_Mode.FromClient;
using BCA_StoryMode.Enums;

namespace hub_client.WindowsAdministrator
{
    public class StoryModeConsoleAdministrator
    {
        public GameClient Client;

        public StoryModeConsoleAdministrator(GameClient client)
        {
            Client = client;
        }

        public void SendGetOpenWorldCharacter()
        {
            Client.Send(PacketType.GetOpenWorldCharacters, new StoryModeClientGetOpenWorldCharacter());
        }
        public void SendGetScenes(ArcID id)
        {
            Client.Send(PacketType.GetScenes, new StoryModeClientAskScene { ArcID = (int)id });
        }
    }
}
